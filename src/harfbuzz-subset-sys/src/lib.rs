#![allow(non_camel_case_types)]

use std::ffi::{c_char, c_void};

pub type hb_ptr_t = *mut c_void;
pub type hb_destroy_func_t = unsafe extern "C" fn(hb_ptr_t);

pub const HB_MEMORY_MODE_DUPLICATE: u32 = 0;
pub const HB_SUBSET_FLAGS_NO_HINTING: u32 = 0x0000_0001;
pub const HB_SUBSET_SETS_DROP_TABLE_TAG: u32 = 3;

unsafe extern "C" {
    pub fn hb_blob_create_or_fail(
        data: *const c_char,
        length: u32,
        mode: u32,
        user_data: hb_ptr_t,
        destroy: Option<hb_destroy_func_t>,
    ) -> hb_ptr_t;
    pub fn hb_blob_destroy(blob: hb_ptr_t);
    pub fn hb_blob_get_data(blob: hb_ptr_t, length: *mut u32) -> *const c_char;

    pub fn hb_face_create(blob: hb_ptr_t, index: u32) -> hb_ptr_t;
    pub fn hb_face_destroy(face: hb_ptr_t);
    pub fn hb_face_reference_blob(face: hb_ptr_t) -> hb_ptr_t;

    pub fn hb_subset_input_create_or_fail() -> hb_ptr_t;
    pub fn hb_subset_input_destroy(input: hb_ptr_t);
    pub fn hb_subset_input_unicode_set(input: hb_ptr_t) -> hb_ptr_t;
    pub fn hb_subset_input_get_flags(input: hb_ptr_t) -> u32;
    pub fn hb_subset_input_set_flags(input: hb_ptr_t, value: u32);
    pub fn hb_subset_input_set(input: hb_ptr_t, set_type: u32) -> hb_ptr_t;
    pub fn hb_subset_or_fail(source: hb_ptr_t, input: hb_ptr_t) -> hb_ptr_t;

    pub fn hb_set_add(set: hb_ptr_t, codepoint: u32);
    pub fn hb_version_string() -> *const c_char;
}
