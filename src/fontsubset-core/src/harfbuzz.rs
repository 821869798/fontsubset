use std::{ffi::CStr, ptr, slice};

use anyhow::{Context, Result, bail};
use harfbuzz_subset_sys as hb;

pub(crate) struct HarfBuzz;

impl HarfBuzz {
    pub(crate) fn new() -> Self {
        Self
    }

    pub(crate) fn display_name(&self) -> String {
        let version = unsafe {
            let value = hb::hb_version_string();
            (!value.is_null()).then(|| CStr::from_ptr(value).to_string_lossy())
        };
        match version {
            Some(version) => format!("HarfBuzz {version} (statically linked)"),
            None => "HarfBuzz (statically linked)".to_string(),
        }
    }

    pub(crate) fn subset_font(
        &self,
        font: &[u8],
        characters: impl IntoIterator<Item = char>,
        strip_hints: bool,
        drop_layout: bool,
    ) -> Result<Vec<u8>> {
        let font_len = u32::try_from(font.len()).context("input font is larger than 4 GiB")?;
        let blob = OwnedHandle::new(
            unsafe {
                hb::hb_blob_create_or_fail(
                    font.as_ptr().cast(),
                    font_len,
                    hb::HB_MEMORY_MODE_DUPLICATE,
                    ptr::null_mut(),
                    None,
                )
            },
            hb::hb_blob_destroy,
            "input blob",
        )?;
        let face = OwnedHandle::new(
            unsafe { hb::hb_face_create(blob.ptr(), 0) },
            hb::hb_face_destroy,
            "input face",
        )?;
        let input = OwnedHandle::new(
            unsafe { hb::hb_subset_input_create_or_fail() },
            hb::hb_subset_input_destroy,
            "subset input",
        )?;

        let unicode_set = unsafe { hb::hb_subset_input_unicode_set(input.ptr()) };
        if unicode_set.is_null() {
            bail!("HarfBuzz failed to provide the subset Unicode set");
        }
        for character in characters {
            unsafe {
                hb::hb_set_add(unicode_set, character as u32);
            }
        }

        if strip_hints {
            let flags = unsafe { hb::hb_subset_input_get_flags(input.ptr()) };
            unsafe {
                hb::hb_subset_input_set_flags(input.ptr(), flags | hb::HB_SUBSET_FLAGS_NO_HINTING);
            }
        }

        if drop_layout {
            let drop_set =
                unsafe { hb::hb_subset_input_set(input.ptr(), hb::HB_SUBSET_SETS_DROP_TABLE_TAG) };
            if drop_set.is_null() {
                bail!("HarfBuzz failed to provide the drop-table set");
            }
            for tag in [*b"GDEF", *b"GPOS", *b"GSUB"] {
                unsafe {
                    hb::hb_set_add(drop_set, u32::from_be_bytes(tag));
                }
            }
        }

        let output_face = OwnedHandle::new(
            unsafe { hb::hb_subset_or_fail(face.ptr(), input.ptr()) },
            hb::hb_face_destroy,
            "subset face",
        )
        .context("HarfBuzz rejected the font or subset options")?;
        let output_blob = OwnedHandle::new(
            unsafe { hb::hb_face_reference_blob(output_face.ptr()) },
            hb::hb_blob_destroy,
            "output blob",
        )?;

        let mut output_len = 0;
        let output_data = unsafe { hb::hb_blob_get_data(output_blob.ptr(), &mut output_len) };
        if output_data.is_null() || output_len == 0 {
            bail!("HarfBuzz produced an empty output font");
        }

        Ok(unsafe { slice::from_raw_parts(output_data.cast(), output_len as usize) }.to_vec())
    }
}

struct OwnedHandle {
    ptr: hb::hb_ptr_t,
    destroy: hb::hb_destroy_func_t,
}

impl OwnedHandle {
    fn new(ptr: hb::hb_ptr_t, destroy: hb::hb_destroy_func_t, description: &str) -> Result<Self> {
        if ptr.is_null() {
            bail!("HarfBuzz failed to allocate {description}");
        }
        Ok(Self { ptr, destroy })
    }

    fn ptr(&self) -> hb::hb_ptr_t {
        self.ptr
    }
}

impl Drop for OwnedHandle {
    fn drop(&mut self) {
        unsafe {
            (self.destroy)(self.ptr);
        }
    }
}
