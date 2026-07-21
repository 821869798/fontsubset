use std::{env, path::PathBuf};

fn main() {
    let manifest_dir = PathBuf::from(env::var_os("CARGO_MANIFEST_DIR").unwrap());
    let source_dir = manifest_dir.join("../../vendor/harfbuzz/src");
    let target = env::var("TARGET").unwrap();

    println!("cargo:rerun-if-changed={}", source_dir.display());

    let mut build = cc::Build::new();
    build
        .cpp(true)
        .warnings(false)
        .include(&source_dir)
        .file(source_dir.join("harfbuzz-subset.cc"));

    if target.contains("msvc") {
        build.flag_if_supported("/std:c++14").flag("/bigobj");
    } else {
        build.flag_if_supported("-std=c++14");
    }

    if target.contains("windows-gnu") {
        build.flag("-Wa,-mbig-obj");
    }
    if !target.contains("windows") {
        build.define("HAVE_PTHREAD", "1");
    }

    build.compile("fontsubset_harfbuzz_subset");
}
