#![cfg_attr(all(windows, not(debug_assertions)), windows_subsystem = "windows")]
#![recursion_limit = "256"]

mod app;
mod i18n;

use std::{
    backtrace::Backtrace,
    fs::OpenOptions,
    io::Write as _,
    panic,
    time::{SystemTime, UNIX_EPOCH},
};

use app::FontSubsetApp;
use gpui::*;
use gpui_component::*;
use gpui_component_assets::Assets;

fn main() {
    install_panic_log();

    let app = gpui_platform::application().with_assets(Assets);

    app.run(move |cx| {
        gpui_component::init(cx);

        let window_options = WindowOptions {
            window_bounds: Some(WindowBounds::centered(size(px(860.), px(520.)), cx)),
            window_min_size: Some(size(px(720.), px(480.))),
            titlebar: Some(TitlebarOptions {
                title: Some("字体裁剪 / Font Subset".into()),
                ..Default::default()
            }),
            ..Default::default()
        };

        cx.spawn(async move |cx| {
            cx.open_window(window_options, |window, cx| {
                let view = cx.new(|cx| FontSubsetApp::new(window, cx));
                cx.new(|cx| Root::new(view, window, cx).bg(cx.theme().background))
            })
            .expect("failed to open Font Subset window");
        })
        .detach();
    });
}

fn install_panic_log() {
    let default_hook = panic::take_hook();
    panic::set_hook(Box::new(move |info| {
        let timestamp = SystemTime::now()
            .duration_since(UNIX_EPOCH)
            .map_or(0, |duration| duration.as_secs());
        let path = std::env::temp_dir().join("fontsubset-gui-crash.log");
        if let Ok(mut log) = OpenOptions::new().create(true).append(true).open(path) {
            let backtrace = Backtrace::force_capture();
            let _ = writeln!(log, "[{timestamp}] {info}\n{backtrace}");
        }
        default_hook(info);
    }));
}
