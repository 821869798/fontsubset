use std::{
    path::{Path, PathBuf},
    process::Command,
    sync::Arc,
    thread,
    time::Duration,
};

use fontsubset_core::{DEFAULT_FILE_REGEX, SubsetRequest, SubsetResult, subset};
use gpui::*;
use gpui_component::{
    ActiveTheme, Disableable,
    button::{Button, ButtonVariants},
    checkbox::Checkbox,
    h_flex,
    input::{Input, InputState},
    v_flex, *,
};
use parking_lot::Mutex;

use crate::i18n::{Locale, Msg};

enum UiMessage {
    Finished(Result<SubsetResult, String>),
}

pub struct FontSubsetApp {
    input_font: Entity<InputState>,
    output_font: Entity<InputState>,
    chars_path: Entity<InputState>,
    file_regex: Entity<InputState>,
    custom_regex: bool,
    strip_hints: bool,
    retain_ascii: bool,
    drop_layout: bool,
    running: bool,
    locale: Locale,
    status: SharedString,
    status_kind: StatusKind,
    inbox: Arc<Mutex<Vec<UiMessage>>>,
}

#[derive(Clone, Copy, Eq, PartialEq)]
enum StatusKind {
    Neutral,
    Success,
    Error,
}

impl FontSubsetApp {
    pub fn new(window: &mut Window, cx: &mut Context<Self>) -> Self {
        let locale = Locale::Zh;
        let input_font = cx.new(|cx| {
            InputState::new(window, cx).placeholder(Msg::InputSourcePlaceholder.get(locale))
        });
        let output_font = cx
            .new(|cx| InputState::new(window, cx).placeholder(Msg::OutputPlaceholder.get(locale)));
        let chars_path = cx.new(|cx| {
            InputState::new(window, cx).placeholder(Msg::CharsPathPlaceholder.get(locale))
        });
        let file_regex = cx.new(|cx| {
            InputState::new(window, cx)
                .default_value(DEFAULT_FILE_REGEX)
                .placeholder(Msg::RegexPlaceholder.get(locale))
        });

        let inbox = Arc::new(Mutex::new(Vec::new()));
        let poll_inbox = Arc::clone(&inbox);
        cx.spawn(async move |this, cx| {
            loop {
                cx.background_executor()
                    .timer(Duration::from_millis(50))
                    .await;
                let messages = {
                    let mut inbox = poll_inbox.lock();
                    std::mem::take(&mut *inbox)
                };
                if messages.is_empty() {
                    continue;
                }
                if this
                    .update(cx, |this, cx| {
                        for message in messages {
                            this.handle_message(message);
                        }
                        cx.notify();
                    })
                    .is_err()
                {
                    break;
                }
            }
        })
        .detach();

        Self {
            input_font,
            output_font,
            chars_path,
            file_regex,
            custom_regex: false,
            strip_hints: true,
            retain_ascii: true,
            drop_layout: false,
            running: false,
            locale,
            status: Msg::Ready.get(locale).into(),
            status_kind: StatusKind::Neutral,
            inbox,
        }
    }

    fn select_input_font(&mut self, window: &mut Window, cx: &mut Context<Self>) {
        let locale = self.locale;
        let Some(path) = rfd::FileDialog::new()
            .set_title(Msg::SelectSourceDialog.get(locale))
            .add_filter(Msg::FontFiles.get(locale), &["ttf", "otf"])
            .pick_file()
        else {
            return;
        };

        self.set_input_value(&self.input_font, path.display().to_string(), window, cx);
        if self.output_font.read(cx).value().trim().is_empty() {
            let suggested = suggested_output_path(&path);
            self.set_input_value(
                &self.output_font,
                suggested.display().to_string(),
                window,
                cx,
            );
        }
        self.set_status(Msg::SourceSelected.get(locale), StatusKind::Neutral, cx);
    }

    fn select_output_font(&mut self, window: &mut Window, cx: &mut Context<Self>) {
        let locale = self.locale;
        let input = PathBuf::from(self.input_font.read(cx).value().as_ref());
        let mut dialog = rfd::FileDialog::new()
            .set_title(Msg::SaveOutputDialog.get(locale))
            .add_filter(Msg::FontFiles.get(locale), &["ttf", "otf"]);
        if let Some(parent) = input.parent().filter(|path| path.is_dir()) {
            dialog = dialog.set_directory(parent);
        }
        if let Some(name) = suggested_output_path(&input).file_name() {
            dialog = dialog.set_file_name(name.to_string_lossy());
        }

        if let Some(path) = dialog.save_file() {
            self.set_input_value(&self.output_font, path.display().to_string(), window, cx);
            self.set_status(Msg::OutputSelected.get(locale), StatusKind::Neutral, cx);
        }
    }

    fn select_chars_path(&mut self, window: &mut Window, cx: &mut Context<Self>) {
        let locale = self.locale;
        if let Some(path) = rfd::FileDialog::new()
            .set_title(Msg::SelectCharsDialog.get(locale))
            .pick_folder()
        {
            self.set_input_value(&self.chars_path, path.display().to_string(), window, cx);
            self.set_status(Msg::CharsSelected.get(locale), StatusKind::Neutral, cx);
        }
    }

    fn set_locale(&mut self, locale: Locale, window: &mut Window, cx: &mut Context<Self>) {
        self.locale = locale;
        for (input, placeholder) in [
            (&self.input_font, Msg::InputSourcePlaceholder),
            (&self.output_font, Msg::OutputPlaceholder),
            (&self.chars_path, Msg::CharsPathPlaceholder),
            (&self.file_regex, Msg::RegexPlaceholder),
        ] {
            input.update(cx, |input, cx| {
                input.set_placeholder(placeholder.get(locale), window, cx);
            });
        }
        if !self.running {
            self.status = Msg::Ready.get(locale).into();
        }
        cx.notify();
    }

    fn set_input_value(
        &self,
        input: &Entity<InputState>,
        value: String,
        window: &mut Window,
        cx: &mut Context<Self>,
    ) {
        input.update(cx, |input, cx| input.set_value(value, window, cx));
    }

    fn start_subset(&mut self, cx: &mut Context<Self>) {
        if self.running {
            return;
        }

        let input = self.input_font.read(cx).value().trim().to_string();
        let output = self.output_font.read(cx).value().trim().to_string();
        let chars_dir = self.chars_path.read(cx).value().trim().to_string();
        let file_regex = self.file_regex.read(cx).value().trim().to_string();
        let locale = self.locale;

        let validation_error = if input.is_empty() {
            Some(Msg::SelectInputError.get(locale))
        } else if output.is_empty() {
            Some(Msg::SelectOutputError.get(locale))
        } else if chars_dir.is_empty() && !self.retain_ascii {
            Some(Msg::CharsOrAsciiError.get(locale))
        } else if file_regex.is_empty() && !chars_dir.is_empty() {
            Some(Msg::RegexEmptyError.get(locale))
        } else {
            None
        };
        if let Some(error) = validation_error {
            self.set_status(error, StatusKind::Error, cx);
            return;
        }

        let request = SubsetRequest {
            input: PathBuf::from(input),
            output: PathBuf::from(output),
            chars_dir: (!chars_dir.is_empty()).then(|| PathBuf::from(chars_dir)),
            file_regex,
            literal_text: None,
            retain_ascii: self.retain_ascii,
            strip_hints: self.strip_hints,
            drop_layout: self.drop_layout,
        };
        let inbox = Arc::clone(&self.inbox);
        self.running = true;
        self.set_status(Msg::Subsetting.get(locale), StatusKind::Neutral, cx);

        thread::spawn(move || {
            let result = subset(&request).map_err(|error| format!("{error:#}"));
            inbox.lock().push(UiMessage::Finished(result));
        });
    }

    fn handle_message(&mut self, message: UiMessage) {
        match message {
            UiMessage::Finished(Ok(result)) => {
                self.running = false;
                self.status = format_success(self.locale, &result).into();
                self.status_kind = StatusKind::Success;
                reveal_output(&result.output);
            }
            UiMessage::Finished(Err(error)) => {
                self.running = false;
                self.status = format!("{}: {error}", Msg::SubsetFailed.get(self.locale)).into();
                self.status_kind = StatusKind::Error;
            }
        }
    }

    fn set_status(
        &mut self,
        status: impl Into<SharedString>,
        kind: StatusKind,
        cx: &mut Context<Self>,
    ) {
        self.status = status.into();
        self.status_kind = kind;
        cx.notify();
    }

    fn render_path_row(
        &self,
        label: &'static str,
        input: &Entity<InputState>,
        button_id: &'static str,
        input_width: Pixels,
        select: impl Fn(&mut Self, &mut Window, &mut Context<Self>) + 'static,
        cx: &mut Context<Self>,
    ) -> AnyElement {
        h_flex()
            .w_full()
            .min_w_0()
            .gap_3()
            .items_center()
            .child(div().w(px(190.)).flex_none().text_sm().child(label))
            .child(
                div()
                    .w(input_width)
                    .flex_none()
                    .child(Input::new(input).disabled(self.running)),
            )
            .child(
                Button::new(button_id)
                    .flex_none()
                    .outline()
                    .label(Msg::Select.get(self.locale))
                    .disabled(self.running)
                    .on_click(cx.listener(move |this, _, window, cx| select(this, window, cx))),
            )
            .into_any_element()
    }

    fn render_options(&self, regex_width: Pixels, cx: &mut Context<Self>) -> AnyElement {
        let locale = self.locale;
        v_flex()
            .w_full()
            .gap_4()
            .child(
                h_flex()
                    .w_full()
                    .min_w_0()
                    .gap_3()
                    .items_center()
                    .child(
                        div()
                            .w(px(190.))
                            .flex_none()
                            .text_sm()
                            .child(Msg::FileRegexMatch.get(locale)),
                    )
                    .child(div().w(regex_width).flex_none().child(
                        Input::new(&self.file_regex).disabled(self.running || !self.custom_regex),
                    ))
                    .child(
                        Checkbox::new("custom-regex")
                            .flex_none()
                            .label(Msg::CustomRegex.get(locale))
                            .checked(self.custom_regex)
                            .disabled(self.running)
                            .on_click(cx.listener(|this, checked, _window, cx| {
                                this.custom_regex = *checked;
                                cx.notify();
                            })),
                    ),
            )
            .child(
                h_flex()
                    .w_full()
                    .gap_5()
                    .items_center()
                    .flex_wrap()
                    .child(
                        Checkbox::new("strip-hints")
                            .label(Msg::StripHinting.get(locale))
                            .checked(self.strip_hints)
                            .disabled(self.running)
                            .on_click(cx.listener(|this, checked, _window, cx| {
                                this.strip_hints = *checked;
                                cx.notify();
                            })),
                    )
                    .child(
                        Checkbox::new("retain-ascii")
                            .label(Msg::RetainAscii.get(locale))
                            .checked(self.retain_ascii)
                            .disabled(self.running)
                            .on_click(cx.listener(|this, checked, _window, cx| {
                                this.retain_ascii = *checked;
                                cx.notify();
                            })),
                    )
                    .child(
                        Checkbox::new("drop-layout")
                            .label(Msg::DropLayout.get(locale))
                            .checked(self.drop_layout)
                            .disabled(self.running)
                            .on_click(cx.listener(|this, checked, _window, cx| {
                                this.drop_layout = *checked;
                                cx.notify();
                            })),
                    )
                    .child(
                        Button::new("start-subset")
                            .primary()
                            .label(if self.running {
                                Msg::Subsetting.get(locale)
                            } else {
                                Msg::StartSubset.get(locale)
                            })
                            .disabled(self.running)
                            .on_click(cx.listener(|this, _, _window, cx| this.start_subset(cx))),
                    ),
            )
            .into_any_element()
    }
}

impl Render for FontSubsetApp {
    fn render(&mut self, window: &mut Window, cx: &mut Context<Self>) -> impl IntoElement {
        let locale = self.locale;
        let viewport = window.viewport_size();
        let path_input_width = px((viewport.width.as_f32() - 342.).max(280.));
        let regex_input_width = px((viewport.width.as_f32() - 400.).max(240.));
        let content_height = px((viewport.height.as_f32() - 174.).max(280.));
        let status_color = match self.status_kind {
            StatusKind::Neutral => cx.theme().muted_foreground,
            StatusKind::Success => cx.theme().success,
            StatusKind::Error => cx.theme().danger,
        };

        v_flex()
            .id("font-subset-root")
            .size_full()
            .bg(cx.theme().background)
            .text_color(cx.theme().foreground)
            .child(
                h_flex()
                    .w_full()
                    .gap_5()
                    .items_center()
                    .flex_none()
                    .h(px(110.))
                    .px_5()
                    .py_4()
                    .border_b_1()
                    .border_color(cx.theme().border)
                    .child(
                        v_flex()
                            .gap_1()
                            .child(
                                div()
                                    .text_lg()
                                    .font_semibold()
                                    .child(Msg::Title.get(locale)),
                            )
                            .child(
                                div()
                                    .text_sm()
                                    .text_color(cx.theme().muted_foreground)
                                    .child(Msg::Subtitle.get(locale)),
                            ),
                    )
                    .child(
                        Button::new("language-switch")
                            .flex_none()
                            .outline()
                            .label(Msg::LanguageSwitch.get(locale))
                            .disabled(self.running)
                            .on_click(cx.listener(|this, _, window, cx| {
                                this.set_locale(this.locale.toggle(), window, cx);
                            })),
                    ),
            )
            .child(
                v_flex()
                    .id("subset-form-scroll")
                    .h(content_height)
                    .flex_none()
                    .w_full()
                    .gap_4()
                    .p_5()
                    .overflow_y_scroll()
                    .child(self.render_path_row(
                        Msg::InputSourceFont.get(locale),
                        &self.input_font,
                        "select-input",
                        path_input_width,
                        Self::select_input_font,
                        cx,
                    ))
                    .child(self.render_path_row(
                        Msg::OutputSubsetFont.get(locale),
                        &self.output_font,
                        "select-output",
                        path_input_width,
                        Self::select_output_font,
                        cx,
                    ))
                    .child(self.render_path_row(
                        Msg::CharsFilesPath.get(locale),
                        &self.chars_path,
                        "select-chars-path",
                        path_input_width,
                        Self::select_chars_path,
                        cx,
                    ))
                    .child(self.render_options(regex_input_width, cx)),
            )
            .child(
                div()
                    .w_full()
                    .h(px(64.))
                    .flex_none()
                    .px_5()
                    .py_3()
                    .border_t_1()
                    .border_color(cx.theme().border)
                    .bg(cx.theme().muted)
                    .text_sm()
                    .text_color(status_color)
                    .child(self.status.clone()),
            )
            .into_any_element()
    }
}

fn suggested_output_path(input: &Path) -> PathBuf {
    let parent = input.parent().unwrap_or_else(|| Path::new(""));
    let stem = input
        .file_stem()
        .and_then(|value| value.to_str())
        .unwrap_or("font");
    let extension = input
        .extension()
        .and_then(|value| value.to_str())
        .unwrap_or("otf");
    parent.join(format!("{stem}-subset.{extension}"))
}

fn format_success(locale: Locale, result: &SubsetResult) -> String {
    match locale {
        Locale::Zh => format!(
            "裁剪成功：{} | 字形 {} -> {} | 大小 {} -> {} 字节（减少 {:.2}%）| 字符 {}/{} | {}",
            result.outline,
            result.original_glyphs,
            result.subset_glyphs,
            result.original_size,
            result.subset_size,
            result.reduction_percent(),
            result.supported_characters,
            result.requested_characters,
            result.output.display()
        ),
        Locale::En => format!(
            "Subset succeeded: {} | glyphs {} -> {} | size {} -> {} bytes ({:.2}% smaller) | characters {}/{} | {}",
            result.outline,
            result.original_glyphs,
            result.subset_glyphs,
            result.original_size,
            result.subset_size,
            result.reduction_percent(),
            result.supported_characters,
            result.requested_characters,
            result.output.display()
        ),
    }
}

fn reveal_output(path: &Path) {
    #[cfg(target_os = "windows")]
    let _ = Command::new("explorer")
        .arg(format!("/select,{}", path.display()))
        .spawn();

    #[cfg(target_os = "macos")]
    let _ = Command::new("open").arg("-R").arg(path).spawn();

    #[cfg(all(unix, not(target_os = "macos")))]
    if let Some(parent) = path.parent() {
        let _ = Command::new("xdg-open").arg(parent).spawn();
    }
}
