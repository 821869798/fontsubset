#[derive(Clone, Copy, Debug, Eq, PartialEq)]
pub enum Locale {
    Zh,
    En,
}

impl Locale {
    pub fn toggle(self) -> Self {
        match self {
            Self::Zh => Self::En,
            Self::En => Self::Zh,
        }
    }
}

#[derive(Clone, Copy, Debug)]
pub enum Msg {
    Title,
    Subtitle,
    LanguageSwitch,
    InputSourceFont,
    InputSourcePlaceholder,
    OutputSubsetFont,
    OutputPlaceholder,
    CharsFilesPath,
    CharsPathPlaceholder,
    FileRegexMatch,
    RegexPlaceholder,
    CustomRegex,
    StripHinting,
    RetainAscii,
    DropLayout,
    StartSubset,
    Subsetting,
    Ready,
    Select,
    SourceSelected,
    OutputSelected,
    CharsSelected,
    SelectInputError,
    SelectOutputError,
    CharsOrAsciiError,
    RegexEmptyError,
    SelectSourceDialog,
    SaveOutputDialog,
    SelectCharsDialog,
    FontFiles,
    SubsetFailed,
}

impl Msg {
    pub fn get(self, locale: Locale) -> &'static str {
        match (self, locale) {
            (Self::Title, Locale::Zh) => "字体裁剪",
            (Self::Title, Locale::En) => "Font Subset",
            (Self::Subtitle, Locale::Zh) => "支持 TrueType、OpenType CFF/CFF2 的字体裁剪工具",
            (Self::Subtitle, Locale::En) => "TrueType and OpenType CFF/CFF2 font subsetter",
            (Self::LanguageSwitch, Locale::Zh) => "中 / EN",
            (Self::LanguageSwitch, Locale::En) => "EN / 中",
            (Self::InputSourceFont, Locale::Zh) => "输入源字体",
            (Self::InputSourceFont, Locale::En) => "Input Source Font",
            (Self::InputSourcePlaceholder, Locale::Zh) => "选择 .ttf 或 .otf 源字体",
            (Self::InputSourcePlaceholder, Locale::En) => "Select a .ttf or .otf source font",
            (Self::OutputSubsetFont, Locale::Zh) => "输出裁剪字体",
            (Self::OutputSubsetFont, Locale::En) => "Output Subset Font",
            (Self::OutputPlaceholder, Locale::Zh) => "选择裁剪字体的输出路径",
            (Self::OutputPlaceholder, Locale::En) => "Choose the subset font output path",
            (Self::CharsFilesPath, Locale::Zh) => "字符集文件目录",
            (Self::CharsFilesPath, Locale::En) => "Input Chars Set Files Path",
            (Self::CharsPathPlaceholder, Locale::Zh) => "选择包含字符集文件的目录",
            (Self::CharsPathPlaceholder, Locale::En) => "Directory containing character set files",
            (Self::FileRegexMatch, Locale::Zh) => "字符集文件正则匹配",
            (Self::FileRegexMatch, Locale::En) => "Chars Set File Regex Match",
            (Self::RegexPlaceholder, Locale::Zh) => "文件路径正则表达式",
            (Self::RegexPlaceholder, Locale::En) => "File path regular expression",
            (Self::CustomRegex, Locale::Zh) => "自定义正则",
            (Self::CustomRegex, Locale::En) => "Custom Regex",
            (Self::StripHinting, Locale::Zh) => "移除 Hinting",
            (Self::StripHinting, Locale::En) => "Strip Hinting",
            (Self::RetainAscii, Locale::Zh) => "保留 ASCII 字符",
            (Self::RetainAscii, Locale::En) => "Retain ASCII Chars",
            (Self::DropLayout, Locale::Zh) => "删除布局表",
            (Self::DropLayout, Locale::En) => "Drop Layout Tables",
            (Self::StartSubset, Locale::Zh) => "开始裁剪",
            (Self::StartSubset, Locale::En) => "Start Subset",
            (Self::Subsetting, Locale::Zh) => "裁剪中...",
            (Self::Subsetting, Locale::En) => "Subsetting...",
            (Self::Ready, Locale::Zh) => "就绪",
            (Self::Ready, Locale::En) => "Ready",
            (Self::Select, Locale::Zh) => "选择",
            (Self::Select, Locale::En) => "Select",
            (Self::SourceSelected, Locale::Zh) => "已选择源字体",
            (Self::SourceSelected, Locale::En) => "Source font selected",
            (Self::OutputSelected, Locale::Zh) => "已选择输出路径",
            (Self::OutputSelected, Locale::En) => "Output path selected",
            (Self::CharsSelected, Locale::Zh) => "已选择字符集目录",
            (Self::CharsSelected, Locale::En) => "Character directory selected",
            (Self::SelectInputError, Locale::Zh) => "请选择输入字体",
            (Self::SelectInputError, Locale::En) => "Select an input font",
            (Self::SelectOutputError, Locale::Zh) => "请选择输出字体",
            (Self::SelectOutputError, Locale::En) => "Select an output font",
            (Self::CharsOrAsciiError, Locale::Zh) => "请选择字符集目录或启用保留 ASCII 字符",
            (Self::CharsOrAsciiError, Locale::En) => {
                "Select a character directory or enable Retain ASCII"
            }
            (Self::RegexEmptyError, Locale::Zh) => "文件正则表达式不能为空",
            (Self::RegexEmptyError, Locale::En) => "The file regex cannot be empty",
            (Self::SelectSourceDialog, Locale::Zh) => "选择源字体",
            (Self::SelectSourceDialog, Locale::En) => "Select Source Font",
            (Self::SaveOutputDialog, Locale::Zh) => "保存裁剪字体",
            (Self::SaveOutputDialog, Locale::En) => "Save Subset Font",
            (Self::SelectCharsDialog, Locale::Zh) => "选择字符集目录",
            (Self::SelectCharsDialog, Locale::En) => "Select Character Set Directory",
            (Self::FontFiles, Locale::Zh) => "字体文件",
            (Self::FontFiles, Locale::En) => "Font files",
            (Self::SubsetFailed, Locale::Zh) => "裁剪失败",
            (Self::SubsetFailed, Locale::En) => "Subset failed",
        }
    }
}
