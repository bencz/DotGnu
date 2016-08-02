/*
 * JpegLib.cs - Implementation of the "DotGNU.Images.JpegLib" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace DotGNU.Images
{

using System;
using System.IO;
using System.Runtime.InteropServices;
using OpenSystem.Platform;

// This class imports definitions from "libjpeg" via PInvoke.  Eventually we
// may want to replace this with a managed JPEG library by compiling "libjpeg"
// directly with "pnetC".  But this should work for now, assuming that the
// underlying OS does indeed have "libjpeg" available.

internal unsafe sealed class JpegLib
{
	public const int JPEG_LIB_VERSION = 62;
	public const int DCTSIZE = 8;
	public const int DCTSIZE2 = 64;
	public const int NUM_QUANT_TBLS = 4;
	public const int NUM_HUFF_TBLS = 4;
	public const int NUM_ARITH_TBLS = 16;
	public const int MAX_COMPS_IN_SCAN = 4;
	public const int MAX_SAMP_FACTOR = 4;
	public const int C_MAX_BLOCKS_IN_MCU = 10;
	public const int D_MAX_BLOCKS_IN_MCU = 10;

	public enum J_COLOR_SPACE
	{
		JCS_UNKNOWN,
		JCS_GRAYSCALE,
		JCS_RGB,
		JCS_YCbCr,
		JCS_CMYK,
		JCS_YCCK
	};

	public enum J_DCT_METHOD
	{
		JDCT_ISLOW,
		JDCT_IFAST,
		JDCT_FLOAT
	};

	public enum J_DITHER_MODE
	{
		JDITHER_NONE,
		JDITHER_ORDERED,
		JDITHER_FS
	};

	[StructLayout(LayoutKind.Sequential)]
	public struct jpeg_compress_struct
	{
		public IntPtr err;
		[NonSerializedAttribute]
		public void *mem;
		[NonSerializedAttribute]
		public void *progress;
		public IntPtr client_data;
		public Int is_decompressor;
		public Int global_state;
		[NonSerializedAttribute]
		public jpeg_destination_mgr *dest;
		public UInt image_width;
		public UInt image_height;
  		public Int input_components;
  		public J_COLOR_SPACE in_color_space;
  		public double input_gamma;
		public Int data_precision;
  		public Int num_components;
		public J_COLOR_SPACE jpeg_color_space;
		[NonSerializedAttribute]
		public void *comp_info;
		[NonSerializedAttribute]
		public void *quant_tbl_ptrs_0;
		[NonSerializedAttribute]
		public void *quant_tbl_ptrs_1;
		[NonSerializedAttribute]
		public void *quant_tbl_ptrs_2;
		[NonSerializedAttribute]
		public void *quant_tbl_ptrs_3;
		[NonSerializedAttribute]
		public void *dc_huff_tbl_ptrs_0;
		[NonSerializedAttribute]
		public void *dc_huff_tbl_ptrs_1;
		[NonSerializedAttribute]
		public void *dc_huff_tbl_ptrs_2;
		[NonSerializedAttribute]
		public void *dc_huff_tbl_ptrs_3;
		[NonSerializedAttribute]
		public void *ac_huff_tbl_ptrs_0;
		[NonSerializedAttribute]
		public void *ac_huff_tbl_ptrs_1;
		[NonSerializedAttribute]
		public void *ac_huff_tbl_ptrs_2;
		[NonSerializedAttribute]
		public void *ac_huff_tbl_ptrs_3;
		public UChar arith_dc_L_0;
		public UChar arith_dc_L_1;
		public UChar arith_dc_L_2;
		public UChar arith_dc_L_3;
		public UChar arith_dc_L_4;
		public UChar arith_dc_L_5;
		public UChar arith_dc_L_6;
		public UChar arith_dc_L_7;
		public UChar arith_dc_L_8;
		public UChar arith_dc_L_9;
		public UChar arith_dc_L_10;
		public UChar arith_dc_L_11;
		public UChar arith_dc_L_12;
		public UChar arith_dc_L_13;
		public UChar arith_dc_L_14;
		public UChar arith_dc_L_15;
		public UChar arith_dc_U_0;
		public UChar arith_dc_U_1;
		public UChar arith_dc_U_2;
		public UChar arith_dc_U_3;
		public UChar arith_dc_U_4;
		public UChar arith_dc_U_5;
		public UChar arith_dc_U_6;
		public UChar arith_dc_U_7;
		public UChar arith_dc_U_8;
		public UChar arith_dc_U_9;
		public UChar arith_dc_U_10;
		public UChar arith_dc_U_11;
		public UChar arith_dc_U_12;
		public UChar arith_dc_U_13;
		public UChar arith_dc_U_14;
		public UChar arith_dc_U_15;
		public UChar arith_dc_K_0;
		public UChar arith_dc_K_1;
		public UChar arith_dc_K_2;
		public UChar arith_dc_K_3;
		public UChar arith_dc_K_4;
		public UChar arith_dc_K_5;
		public UChar arith_dc_K_6;
		public UChar arith_dc_K_7;
		public UChar arith_dc_K_8;
		public UChar arith_dc_K_9;
		public UChar arith_dc_K_10;
		public UChar arith_dc_K_11;
		public UChar arith_dc_K_12;
		public UChar arith_dc_K_13;
		public UChar arith_dc_K_14;
		public UChar arith_dc_K_15;
		public Int num_scans;
		[NonSerializedAttribute]
  		public void *scan_info;
  		public Int raw_data_in;
  		public Int arith_code;
  		public Int optimize_coding;
  		public Int CCIR601_sampling;
  		public Int smoothing_factor;
  		public J_DCT_METHOD dct_method;
  		public UInt restart_interval;
  		public Int restart_in_rows;
  		public Int write_JFIF_header;
  		public UChar JFIF_major_version;
  		public UChar JFIF_minor_version;
  		public UChar density_unit;
  		public UShort X_density;
  		public UShort Y_density;
  		public Int write_Adobe_marker;
  		public UInt next_scanline;
  		public Int progressive_mode;
  		public Int max_h_samp_factor;
  		public Int max_v_samp_factor;
  		public UInt total_iMCU_rows;
  		public Int comps_in_scan;
		[NonSerializedAttribute]
  		public void *cur_comp_info_0;
		[NonSerializedAttribute]
  		public void *cur_comp_info_1;
		[NonSerializedAttribute]
  		public void *cur_comp_info_2;
		[NonSerializedAttribute]
  		public void *cur_comp_info_3;
  		public UInt MCUs_per_row;
  		public UInt MCU_rows_in_scan;
		public Int blocks_in_MCU;
  		public Int MCU_membership_0;
  		public Int MCU_membership_1;
  		public Int MCU_membership_2;
  		public Int MCU_membership_3;
  		public Int MCU_membership_4;
  		public Int MCU_membership_5;
  		public Int MCU_membership_6;
  		public Int MCU_membership_7;
  		public Int MCU_membership_8;
  		public Int MCU_membership_9;
  		public Int Ss, Se, Ah, Al;
		[NonSerializedAttribute]
  		public void *master;
		[NonSerializedAttribute]
  		public void *main;
		[NonSerializedAttribute]
  		public void *prep;
		[NonSerializedAttribute]
  		public void *coef;
		[NonSerializedAttribute]
		public void *marker;
		[NonSerializedAttribute]
  		public void *cconvert;
		[NonSerializedAttribute]
  		public void *downsample;
		[NonSerializedAttribute]
  		public void *fdct;
		[NonSerializedAttribute]
  		public void *entropy;
		[NonSerializedAttribute]
  		public void *script_space;
  		public Int script_space_size;

	}; // struct jpeg_compress_struct

	[StructLayout(LayoutKind.Sequential)]
	public struct jpeg_decompress_struct
	{
		public IntPtr err;
		[NonSerializedAttribute]
		public void *mem;
		[NonSerializedAttribute]
		public void *progress;
		public IntPtr client_data;
		public Int is_decompressor;
		public Int global_state;
		[NonSerializedAttribute]
  		public jpeg_source_mgr *src;
  		public UInt image_width;
  		public UInt image_height;
  		public Int num_components;
  		public J_COLOR_SPACE jpeg_color_space;
  		public J_COLOR_SPACE out_color_space;
  		public UInt scale_num, scale_denom;
  		public double output_gamma;
  		public Int buffered_image;
  		public Int raw_data_out;
  		public J_DCT_METHOD dct_method;
  		public Int do_fancy_upsampling;
  		public Int do_block_smoothing;
  		public Int quantize_colors;
  		public J_DITHER_MODE dither_mode;
  		public Int two_pass_quantize;
  		public Int desired_number_of_colors;
  		public Int enable_1pass_quant;
  		public Int enable_external_quant;
  		public Int enable_2pass_quant;
  		public UInt output_width;
  		public UInt output_height;
  		public Int out_color_components;
  		public Int output_components;
  		public Int rec_outbuf_height;
  		public Int actual_number_of_colors;
		[NonSerializedAttribute]
  		public void *colormap;
  		public UInt output_scanline;
  		public Int input_scan_number;
  		public UInt input_iMCU_row;
  		public Int output_scan_number;
  		public UInt output_iMCU_row;
		[NonSerializedAttribute]
		public void *coef_bits;
		[NonSerializedAttribute]
  		public void *quant_tbl_ptrs_0;
		[NonSerializedAttribute]
  		public void *quant_tbl_ptrs_1;
		[NonSerializedAttribute]
  		public void *quant_tbl_ptrs_2;
		[NonSerializedAttribute]
  		public void *quant_tbl_ptrs_3;
		[NonSerializedAttribute]
  		public void *dc_huff_tbl_ptrs_0;
		[NonSerializedAttribute]
  		public void *dc_huff_tbl_ptrs_1;
		[NonSerializedAttribute]
  		public void *dc_huff_tbl_ptrs_2;
		[NonSerializedAttribute]
  		public void *dc_huff_tbl_ptrs_3;
		[NonSerializedAttribute]
  		public void *ac_huff_tbl_ptrs_0;
 		[NonSerializedAttribute]
  	 	public void *ac_huff_tbl_ptrs_1;
		[NonSerializedAttribute]
  		public void *ac_huff_tbl_ptrs_2;
 		[NonSerializedAttribute]
  	 	public void *ac_huff_tbl_ptrs_3;
  		public Int data_precision;
 		[NonSerializedAttribute]
  	 	public void *comp_info;
  		public Int progressive_mode;
  		public Int arith_code;
		public UChar arith_dc_L_0;
		public UChar arith_dc_L_1;
		public UChar arith_dc_L_2;
		public UChar arith_dc_L_3;
		public UChar arith_dc_L_4;
		public UChar arith_dc_L_5;
		public UChar arith_dc_L_6;
		public UChar arith_dc_L_7;
		public UChar arith_dc_L_8;
		public UChar arith_dc_L_9;
		public UChar arith_dc_L_10;
		public UChar arith_dc_L_11;
		public UChar arith_dc_L_12;
		public UChar arith_dc_L_13;
		public UChar arith_dc_L_14;
		public UChar arith_dc_L_15;
		public UChar arith_dc_U_0;
		public UChar arith_dc_U_1;
		public UChar arith_dc_U_2;
		public UChar arith_dc_U_3;
		public UChar arith_dc_U_4;
		public UChar arith_dc_U_5;
		public UChar arith_dc_U_6;
		public UChar arith_dc_U_7;
		public UChar arith_dc_U_8;
		public UChar arith_dc_U_9;
		public UChar arith_dc_U_10;
		public UChar arith_dc_U_11;
		public UChar arith_dc_U_12;
		public UChar arith_dc_U_13;
		public UChar arith_dc_U_14;
		public UChar arith_dc_U_15;
		public UChar arith_dc_K_0;
		public UChar arith_dc_K_1;
		public UChar arith_dc_K_2;
		public UChar arith_dc_K_3;
		public UChar arith_dc_K_4;
		public UChar arith_dc_K_5;
		public UChar arith_dc_K_6;
		public UChar arith_dc_K_7;
		public UChar arith_dc_K_8;
		public UChar arith_dc_K_9;
		public UChar arith_dc_K_10;
		public UChar arith_dc_K_11;
		public UChar arith_dc_K_12;
		public UChar arith_dc_K_13;
		public UChar arith_dc_K_14;
		public UChar arith_dc_K_15;
  		public UInt restart_interval;
  		public Int saw_JFIF_marker;
  		public UChar JFIF_major_version;
  		public UChar JFIF_minor_version;
  		public UChar density_unit;
  		public UShort X_density;
  		public UShort Y_density;
  		public Int saw_Adobe_marker;
  		public UChar Adobe_transform;
  		public Int CCIR601_sampling;
 		[NonSerializedAttribute]
 		public void *marker_list;
  		public Int max_h_samp_factor;
  		public Int max_v_samp_factor;
  		public Int min_DCT_scaled_size;
  		public UInt total_iMCU_rows;
 		[NonSerializedAttribute]
 		public void *sample_range_limit;
  		public Int comps_in_scan;
 		[NonSerializedAttribute]
 		public void *cur_comp_info_0;
 		[NonSerializedAttribute]
 		public void *cur_comp_info_1;
		[NonSerializedAttribute]
  		public void *cur_comp_info_2;
 		[NonSerializedAttribute]
 		public void *cur_comp_info_3;
  		public UInt MCUs_per_row;
  		public UInt MCU_rows_in_scan;
  		public Int blocks_in_MCU;
  		public Int MCU_membership_0;
  		public Int MCU_membership_1;
  		public Int MCU_membership_2;
  		public Int MCU_membership_3;
  		public Int MCU_membership_4;
  		public Int MCU_membership_5;
  		public Int MCU_membership_6;
  		public Int MCU_membership_7;
  		public Int MCU_membership_8;
  		public Int MCU_membership_9;
  		public Int Ss, Se, Ah, Al;
  		public Int unread_marker;
 		[NonSerializedAttribute]
   		public void *master;
 		[NonSerializedAttribute]
   		public void *main;
 		[NonSerializedAttribute]
   		public void *coef;
 		[NonSerializedAttribute]
   		public void *post;
 		[NonSerializedAttribute]
   		public void *inputctl;
 		[NonSerializedAttribute]
   		public void *marker;
 		[NonSerializedAttribute]
   		public void *entropy;
 		[NonSerializedAttribute]
   		public void *idct;
 		[NonSerializedAttribute]
   		public void *upsample;
 		[NonSerializedAttribute]
   		public void *cconvert;
 		[NonSerializedAttribute]
   		public void *cquantize;

	}; // struct jpeg_decompress_struct

	public delegate void init_source_type
			(ref jpeg_decompress_struct cinfo);
	public delegate Int fill_input_buffer_type
			(ref jpeg_decompress_struct cinfo);
	public delegate void skip_input_data_type
			(ref jpeg_decompress_struct cinfo, Long num_bytes);
	public delegate Int resync_to_restart_type
			(ref jpeg_decompress_struct cinfo, Int desired);
	public delegate void term_source_type
			(ref jpeg_decompress_struct cinfo);

	[StructLayout(LayoutKind.Sequential)]
	public struct jpeg_source_mgr
	{
		public IntPtr next_input_byte;
		public size_t bytes_in_buffer;
		public init_source_type init_source;
		public fill_input_buffer_type fill_input_buffer;
		public skip_input_data_type skip_input_data;
		public resync_to_restart_type resync_to_restart;
		public term_source_type term_source;

	}; // struct jpeg_source_mgr

	public delegate void init_destination_type
			(ref jpeg_compress_struct cinfo);
	public delegate Int empty_output_buffer_type
			(ref jpeg_compress_struct cinfo);
	public delegate void term_destination_type
			(ref jpeg_compress_struct cinfo);

	[StructLayout(LayoutKind.Sequential)]
	public struct jpeg_destination_mgr
	{
  		public IntPtr next_output_byte;
  		public size_t free_in_buffer;
		public init_destination_type init_destination;
		public empty_output_buffer_type empty_output_buffer;
		public term_destination_type term_destination;

	}; // struct jpeg_destination_mgr

	[DllImport("jpeg")]
	extern public static void jpeg_CreateCompress
			(ref jpeg_compress_struct cinfo, Int version, size_t structsize);

	public static void jpeg_create_compress(ref jpeg_compress_struct cinfo)
			{
				jpeg_CreateCompress(ref cinfo, (Int)JPEG_LIB_VERSION,
									(size_t)(sizeof(jpeg_compress_struct)));
			}

	[DllImport("jpeg")]
	extern public static void jpeg_CreateDecompress
			(ref jpeg_decompress_struct cinfo, Int version, size_t structsize);

	public static void jpeg_create_decompress(ref jpeg_decompress_struct cinfo)
			{
				jpeg_CreateDecompress(ref cinfo, (Int)JPEG_LIB_VERSION,
									  (size_t)(sizeof(jpeg_decompress_struct)));
			}

	[DllImport("jpeg")]
	extern public static void jpeg_destroy_compress
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_destroy_decompress
				(ref jpeg_decompress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_set_defaults
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_set_colorspace
				(ref jpeg_compress_struct cinfo, J_COLOR_SPACE colorspace);

	[DllImport("jpeg")]
	extern public static void jpeg_default_colorspace
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_set_quality
				(ref jpeg_compress_struct cinfo, Int quality,
				 Int force_baseline);

	[DllImport("jpeg")]
	extern public static void jpeg_set_linear_quality
				(ref jpeg_compress_struct cinfo, Int scale_factor,
				 Int force_baseline);

	[DllImport("jpeg")]
	extern public static void jpeg_add_quant_table
				(ref jpeg_compress_struct cinfo, Int which_tbl,
				 void *basic_table, Int scale_factor, Int force_baseline);

	[DllImport("jpeg")]
	extern public static Int jpeg_quality_scaling(Int quality);

	[DllImport("jpeg")]
	extern public static void jpeg_simple_progression
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_suppress_tables
				(ref jpeg_compress_struct cinfo, Int suppress);

	[DllImport("jpeg")]
	extern public static void *jpeg_alloc_quant_table
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void *jpeg_alloc_quant_table
				(ref jpeg_decompress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void *jpeg_alloc_huff_table
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void *jpeg_alloc_huff_table
				(ref jpeg_decompress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_start_compress
				(ref jpeg_compress_struct cinfo, Int write_all_tables);

	// Note: can only be used 1 scanline at a time.
	[DllImport("jpeg")]
	extern public static UInt jpeg_write_scanlines
				(ref jpeg_compress_struct cinfo,
				 ref IntPtr scanline, UInt num_lines);

	[DllImport("jpeg")]
	extern public static void jpeg_finish_compress
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_write_tables
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_calc_output_dimensions
				(ref jpeg_decompress_struct cinfo);

	[DllImport("jpeg")]
	extern public static Int jpeg_read_header
				(ref jpeg_decompress_struct cinfo, Int require_image);

	public const Int JPEG_SUSPENDED = (Int)0;
	public const Int JPEG_HEADER_OK = (Int)1;
	public const Int JPEG_HEADER_TABLES_ONLY = (Int)2;

	[DllImport("jpeg")]
	extern public static Int jpeg_start_decompress
				(ref jpeg_decompress_struct cinfo);

	// Note: can only be used 1 scanline at a time.
	[DllImport("jpeg")]
	extern public static UInt jpeg_read_scanlines
				(ref jpeg_decompress_struct cinfo,
				 ref IntPtr scanline, UInt max_lines);

	[DllImport("jpeg")]
	extern public static Int jpeg_finish_decompress
				(ref jpeg_decompress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_abort_compress
				(ref jpeg_compress_struct cinfo);

	[DllImport("jpeg")]
	extern public static void jpeg_abort_decompress
				(ref jpeg_decompress_struct cinfo);

	[DllImport("jpeg")]
	extern public static Int jpeg_resync_to_restart
				(ref jpeg_decompress_struct cinfo, Int desired);

	[DllImport("jpeg")]
	extern public static IntPtr jpeg_std_error(IntPtr err);

	// State data for stream processing.
	private class StreamState
	{
		public IntPtr buf;
		public byte[] buffer;
		public Stream stream;
		public bool sawEOF;

	}; // class StreamState

	// Get the stream state for a decompress structure.
	private static StreamState GetStreamState(ref jpeg_decompress_struct cinfo)
			{
				GCHandle handle = (GCHandle)(cinfo.client_data);
				return (StreamState)(handle.Target);
			}

	// Initialize a stream data source.
	private static void init_source(ref jpeg_decompress_struct cinfo)
			{
				// Nothing to do here: already initialized.
			}

	// Fill an input buffer from a stream.
	private static Int fill_input_buffer(ref jpeg_decompress_struct cinfo)
			{
				int len;
				StreamState state = GetStreamState(ref cinfo);
				if(!(state.sawEOF))
				{
					len = state.stream.Read
						(state.buffer, 0, state.buffer.Length);
					if(len > 0)
					{
						Marshal.Copy(state.buffer, 0, state.buf, len);
						cinfo.src->next_input_byte = state.buf;
						cinfo.src->bytes_in_buffer = (size_t)len;
						return (Int)1;
					}
					state.sawEOF = true;
				}

				// Insert an EOI marker to indicate end of stream to "libjpeg".
				Marshal.WriteByte(state.buf, 0, (byte)0xFF);
				Marshal.WriteByte(state.buf, 1, (byte)0xD9);
				cinfo.src->next_input_byte = state.buf;
				cinfo.src->bytes_in_buffer = (size_t)2;
				return (Int)1;
			}

	// Skip data in an input stream.
	private static void skip_input_data
				(ref jpeg_decompress_struct cinfo, Long num_bytes)
			{
#if __CSCC__
				jpeg_source_mgr *src = cinfo.src;
				int num = (int)num_bytes;
				if(num > 0)
				{
					while(num > (int)(src->bytes_in_buffer))
					{
						num -= (int)(src->bytes_in_buffer);
						fill_input_buffer(ref cinfo);
					}
					src->next_input_byte =
						new IntPtr((src->next_input_byte.ToInt64()) + num);
					src->bytes_in_buffer =
						(size_t)(((int)(src->bytes_in_buffer)) - num);
				}
#endif
			}

	// Terminate an input source.
	private static void term_source(ref jpeg_decompress_struct cinfo)
			{
				// Nothing to do here.
			}

	// Convert a stream into a source manager.
	public static void StreamToSourceManager
				(ref jpeg_decompress_struct cinfo, Stream stream,
				 byte[] prime, int primeLen)
			{
				// Allocate a state structure and store it in "cinfo".
				IntPtr buf = Marshal.AllocHGlobal(4096);
				StreamState state = new StreamState();
				state.buf = buf;
				state.buffer = new byte [4096];
				state.stream = stream;
				cinfo.client_data = (IntPtr)(GCHandle.Alloc(state));

				// We prime the input buffer with the JPEG magic number
				// if some higher-level process has already read it.
				int len;
				if(prime != null)
				{
					len = primeLen;
					Marshal.Copy(prime, 0, buf, len);
				}
				else
				{
					len = 0;
				}

				// Create the managed version of "jpeg_source_mgr".
				jpeg_source_mgr mgr = new jpeg_source_mgr();
				mgr.next_input_byte = buf;
				mgr.bytes_in_buffer = (size_t)len;
				mgr.init_source = new init_source_type(init_source);
				mgr.fill_input_buffer =
					new fill_input_buffer_type(fill_input_buffer);
				mgr.skip_input_data =
					new skip_input_data_type(skip_input_data);
				mgr.resync_to_restart =
					new resync_to_restart_type(jpeg_resync_to_restart);
				mgr.term_source =
					new term_source_type(term_source);

				// Convert it into the unmanaged version and store it.
#if __CSCC__
				IntPtr umgr = Marshal.AllocHGlobal(sizeof(jpeg_source_mgr));
				Marshal.StructureToPtr(mgr, umgr, false);
				cinfo.src = (jpeg_source_mgr *)umgr;
#endif
			}

	// Free a source manager.
	public static void FreeSourceManager(ref jpeg_decompress_struct cinfo)
			{
				GCHandle handle = (GCHandle)(cinfo.client_data);
				StreamState state = (StreamState)(handle.Target);
				Marshal.FreeHGlobal(state.buf);
				handle.Free();
				Marshal.FreeHGlobal((IntPtr)(cinfo.src));
				cinfo.client_data = IntPtr.Zero;
				cinfo.src = null;
			}

	// Get the stream state for a compress structure.
	private static StreamState GetStreamState(ref jpeg_compress_struct cinfo)
			{
				GCHandle handle = (GCHandle)(cinfo.client_data);
				return (StreamState)(handle.Target);
			}

	// Initialize a stream data destination.
	private static void init_destination(ref jpeg_compress_struct cinfo)
			{
				// Nothing to do here: already initialized.
			}

	// Empty an output buffer to a stream.
	private static Int empty_output_buffer(ref jpeg_compress_struct cinfo)
			{
				int len;
				StreamState state = GetStreamState(ref cinfo);
				len = state.buffer.Length;
				Marshal.Copy(state.buf, state.buffer, 0, len);
				state.stream.Write(state.buffer, 0, len);
				cinfo.dest->next_output_byte = state.buf;
				cinfo.dest->free_in_buffer = (size_t)len;
				return (Int)1;
			}

	// Terminate an output destination.
	private static void term_destination(ref jpeg_compress_struct cinfo)
			{
				// Nothing to do here.
			}

	// Convert a stream into a destination manager.
	public static void StreamToDestinationManager
				(ref jpeg_compress_struct cinfo, Stream stream)
			{
				// Allocate a state structure and store it in "cinfo".
				IntPtr buf = Marshal.AllocHGlobal(4096);
				StreamState state = new StreamState();
				state.buf = buf;
				state.buffer = new byte [4096];
				state.stream = stream;
				cinfo.client_data = (IntPtr)(GCHandle.Alloc(state));

				// Create the managed version of "jpeg_destination_mgr".
				jpeg_destination_mgr mgr = new jpeg_destination_mgr();
				mgr.next_output_byte = buf;
				mgr.free_in_buffer = (size_t)4096;
				mgr.init_destination =
					new init_destination_type(init_destination);
				mgr.empty_output_buffer =
					new empty_output_buffer_type(empty_output_buffer);
				mgr.term_destination =
					new term_destination_type(term_destination);

				// Convert it into the unmanaged version and store it.
#if __CSCC__
				IntPtr umgr = Marshal.AllocHGlobal
					(sizeof(jpeg_destination_mgr));
				Marshal.StructureToPtr(mgr, umgr, false);
				cinfo.dest = (jpeg_destination_mgr *)umgr;
#endif
			}

	// Free a destination manager.
	public static void FreeDestinationManager(ref jpeg_compress_struct cinfo)
			{
				GCHandle handle = (GCHandle)(cinfo.client_data);
				StreamState state = (StreamState)(handle.Target);
				Marshal.FreeHGlobal(state.buf);
				handle.Free();
				Marshal.FreeHGlobal((IntPtr)(cinfo.dest));
				cinfo.client_data = IntPtr.Zero;
				cinfo.dest = null;
			}

	// Create a standard error handler.
	public static IntPtr CreateErrorHandler()
			{
				IntPtr err = Marshal.AllocHGlobal(512);
				return jpeg_std_error(err);
			}

	// Free a standard error handler.
	public static void FreeErrorHandler(IntPtr err)
			{
				Marshal.FreeHGlobal(err);
			}

	// Determine if the "libjpeg" library is present.
	public static bool JpegLibraryPresent()
			{
				try
				{
					// Call an innocuous function, which will cause an
					// exception throw if the library is not present.
					jpeg_quality_scaling(0);
					return true;
				}
				catch(Exception)
				{
					return false;
				}
			}

}; // class JpegLib

}; // namespace DotGNU.Images
