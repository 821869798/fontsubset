# FontSubset

[English](README.en.md) | 中文

使用 Rust、[HarfBuzz Subset](https://harfbuzz.github.io/harfbuzz-hb-subset.html)、
[GPUI](https://github.com/zed-industries/zed/tree/main/crates/gpui) 和
[GPUI Component](https://github.com/longbridge/gpui-component) 实现的桌面字体裁剪工具。
PostScript 轮廓会被直接裁剪，不会转换为 TrueType。

## 功能

- 支持 `.ttf` 和 `.otf`
- 支持 TrueType `glyf` 轮廓
- 支持 PostScript CFF、CID-keyed CFF 和可变 CFF2 轮廓
- 递归扫描字符集目录，并按正则表达式匹配文本文件
- 可保留可见 ASCII 字符，可移除 TrueType Hinting
- 默认保留 `GSUB`、`GPOS` 和 `GDEF` 等复杂布局表
- 严格验证输出 cmap 只包含源字体支持的目标字符
- GUI 和 console 共用同一套 `fontsubset-core` 实现
- GUI 支持中文和英文实时切换，耗时裁剪在后台线程执行

独立 Type 1 `.pfa` 和 `.pfb` 文件不是 OpenType 字体，当前不直接接受。
请先转换为 OpenType CFF。OpenType CFF/CFF2 会直接裁剪，不需要转成 TTF。

## 截图

![FontSubset 中文界面](docs/screenshot-ui.png)

## GUI 使用

1. 选择输入 `.ttf` 或 `.otf` 字体。
2. 选择输出字体路径。
3. 选择包含字符集文件的目录。
4. 按需启用自定义正则、保留 ASCII、移除 Hinting 或删除布局表。
5. 点击“开始裁剪”。

标题区的 **中 / EN** 按钮可以切换界面语言。布局表默认保留；只有明确启用
“删除布局表”后才会移除 `GSUB`、`GPOS` 和 `GDEF`。

## Console 使用

从目录中的匹配文件收集字符：

```shell
fontsubset-console -c examples/input_text -a examples/IMPACT.TTF output.ttf
```

直接指定文本：

```shell
fontsubset-console --text "你好，世界" input.otf output.otf
```

兼容旧参数名称 `--charsfile` 和 `--strip`。使用 `-s` 或 `--strip-hints`
移除 TrueType Hinting；使用 `--drop-layout` 主动删除布局表。

## 自包含 HarfBuzz

HarfBuzz 11.2.0 Subset 已从 vendored 源码静态编译进 GUI 和 console。
Windows、Linux 和 macOS 用户都不需要安装 HarfBuzz、复制 DLL 或设置环境变量。
两个可执行文件会各自增加约 2 MB，但发布包可以直接运行，不依赖系统 HarfBuzz 版本。

## 构建

需要支持 Rust 2024 edition 的稳定版工具链。Windows 需要 MSVC 和 Windows SDK。
首次构建会下载 GPUI/Zed Git 依赖，耗时会较长。

```shell
cargo build --release --workspace
```

产物：

```text
target/release/fontsubset-console.exe
target/release/fontsubset-gui.exe
```

运行 GUI：

```shell
cargo run -p fontsubset-gui
```

## 项目结构

```text
src/
  harfbuzz-subset-sys/  # HarfBuzz Subset 静态 FFI 和 C++ 构建
  fontsubset-core/       # 字符收集、HarfBuzz 裁剪和严格结果验证
  fontsubset-console/    # 命令行前端
  fontsubset-gui/        # GPUI 界面和中英文资源
legacy/                  # 原 C#/Avalonia 实现及其构建文件
examples/                # 示例字体和字符集文件
vendor/harfbuzz/         # 固定版本 HarfBuzz 11.2.0 源码和许可证
```

## 测试

```shell
cargo fmt --all -- --check
cargo test --locked --workspace
cargo clippy --locked --workspace --all-targets -- -D warnings
```

项目已使用 TrueType、普通 CFF、CID-keyed CFF 和可变 CFF2 字体进行真实裁剪验证。

## 发布

推送 `v*` tag 会触发 GitHub Actions，构建 Windows、Linux 和 macOS 压缩包，
生成 SHA-256 校验文件并发布 GitHub Release。tag 版本必须与根目录
`Cargo.toml` 的 workspace 版本一致。

```shell
git tag v0.1.0
git push origin v0.1.0
```

GUI 和 console 会发布为独立下载包：

- `fontsubset-gui-windows-x64.zip`
- `fontsubset-console-windows-x64.zip`
- `fontsubset-gui-linux-x64.tar.gz`
- `fontsubset-console-linux-x64.tar.gz`
- `fontsubset-gui-linux-arm64.tar.gz`
- `fontsubset-console-linux-arm64.tar.gz`
- `fontsubset-gui-macos-arm64.zip`
- `fontsubset-console-macos-arm64.tar.gz`

macOS GUI 包中是标准 `FontSubset.app`，并进行了 ad-hoc codesign，但没有
Apple Developer ID 签名和公证。
如果 Gatekeeper 阻止首次运行，可以先尝试右键点击应用并选择“打开”，或者执行：

```shell
xattr -dr com.apple.quarantine FontSubset.app
open FontSubset.app
```

`xattr -cr FontSubset.app` 也能清除限制，但会删除应用上的全部扩展属性；只删除
`com.apple.quarantine` 更精确。要做到下载后直接双击且不出现警告，仍然需要
Developer ID 签名和 Apple notarization。

## License

[MIT](LICENSE)
