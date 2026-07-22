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
    CustomRegexHelp,
    StripHinting,
    StripHintingHelp,
    RetainAscii,
    RetainAsciiHelp,
    ShowOptionHelp,
    StartSubset,
    Subsetting,
    Ready,
    Select,
    SourceSelected,
    FontDropped,
    DropFontError,
    DropWhileRunning,
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
            (Self::Subtitle, Locale::Zh) => {
                "支持 TrueType 与 PostScript（OpenType CFF/CFF2）字体裁剪"
            }
            (Self::Subtitle, Locale::En) => {
                "Supports TrueType and PostScript (OpenType CFF/CFF2) fonts"
            }
            (Self::LanguageSwitch, Locale::Zh) => "中 / EN",
            (Self::LanguageSwitch, Locale::En) => "EN / 中",
            (Self::InputSourceFont, Locale::Zh) => "输入源字体",
            (Self::InputSourceFont, Locale::En) => "Input Source Font",
            (Self::InputSourcePlaceholder, Locale::Zh) => "选择或拖入 .ttf / .otf 源字体",
            (Self::InputSourcePlaceholder, Locale::En) => {
                "Select or drop a .ttf / .otf source font"
            }
            (Self::OutputSubsetFont, Locale::Zh) => "输出裁剪字体",
            (Self::OutputSubsetFont, Locale::En) => "Output Subset Font",
            (Self::OutputPlaceholder, Locale::Zh) => "选择裁剪字体的输出路径",
            (Self::OutputPlaceholder, Locale::En) => "Choose the subset font output path",
            (Self::CharsFilesPath, Locale::Zh) => "字符集文件目录（可选）",
            (Self::CharsFilesPath, Locale::En) => "Chars Directory (Optional)",
            (Self::CharsPathPlaceholder, Locale::Zh) => "可选：选择包含字符集文件的目录",
            (Self::CharsPathPlaceholder, Locale::En) => {
                "Optional: directory containing character set files"
            }
            (Self::FileRegexMatch, Locale::Zh) => "字符集文件正则匹配",
            (Self::FileRegexMatch, Locale::En) => "Chars Set File Regex Match",
            (Self::RegexPlaceholder, Locale::Zh) => "文件路径正则表达式",
            (Self::RegexPlaceholder, Locale::En) => "File path regular expression",
            (Self::CustomRegex, Locale::Zh) => "自定义正则",
            (Self::CustomRegex, Locale::En) => "Custom Regex",
            (Self::CustomRegexHelp, Locale::Zh) => {
                "启用后可以编辑左侧的正则表达式。程序会用它匹配字符集目录中文件的完整路径，只读取匹配到的文件；默认规则匹配 .txt、.lua 和 .asset 文件。"
            }
            (Self::CustomRegexHelp, Locale::En) => {
                "Enable this to edit the regular expression on the left. It is matched against each full file path in the character directory, and only matching files are read. The default matches .txt, .lua, and .asset files."
            }
            (Self::StripHinting, Locale::Zh) => "移除 Hinting",
            (Self::StripHinting, Locale::En) => "Strip Hinting",
            (Self::StripHintingHelp, Locale::Zh) => {
                "删除字体中用于小字号像素对齐的 TrueType 或 CFF/CFF2 Hinting 指令，可以进一步减小文件体积；但在旧系统或低 DPI 屏幕上，小字号文字可能变得稍模糊。默认关闭。"
            }
            (Self::StripHintingHelp, Locale::En) => {
                "Remove TrueType or CFF/CFF2 hinting instructions used to align small text to the pixel grid. This can further reduce file size, but small text may look slightly blurrier on older systems or low-DPI displays. Disabled by default."
            }
            (Self::RetainAscii, Locale::Zh) => "保留 ASCII 字符",
            (Self::RetainAscii, Locale::En) => "Retain ASCII Chars",
            (Self::RetainAsciiHelp, Locale::Zh) => {
                "除字符集文件中的字符外，额外保留 U+0020 至 U+007E，包括英文大小写字母、数字、空格和常用英文符号。关闭后只保留字符集文件中实际出现的字符。"
            }
            (Self::RetainAsciiHelp, Locale::En) => {
                "Also retain U+0020 through U+007E in addition to characters found in the character files. This includes English letters, digits, spaces, and common punctuation. When disabled, only characters actually found in the character files are retained."
            }
            (Self::ShowOptionHelp, Locale::Zh) => "查看选项详细说明",
            (Self::ShowOptionHelp, Locale::En) => "Show option details",
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
            (Self::FontDropped, Locale::Zh) => "已拖入源字体",
            (Self::FontDropped, Locale::En) => "Source font dropped",
            (Self::DropFontError, Locale::Zh) => "请拖入有效的 .ttf 或 .otf 字体文件",
            (Self::DropFontError, Locale::En) => "Drop a valid .ttf or .otf font file",
            (Self::DropWhileRunning, Locale::Zh) => "裁剪进行中，无法更换源字体",
            (Self::DropWhileRunning, Locale::En) => {
                "Cannot change the source font while subsetting"
            }
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
