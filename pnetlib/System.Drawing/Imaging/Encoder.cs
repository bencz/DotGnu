/*
 * Encoder.cs - Implementation of the
 *			"System.Drawing.Imaging.Encoder" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Drawing.Imaging
{

#if !ECMA_COMPAT

public sealed class Encoder
{
	// Internal state.
	private Guid guid;

	// Standard encoders.
	public static readonly Encoder ChrominanceTable =
			new Encoder
				(new Guid("{f2e455dc-09b3-4316-8260-676ada32481c}"));
	public static readonly Encoder ColorDepth =
			new Encoder
				(new Guid("{66087055-ad66-4c7c-9a18-38a2310b8337}"));
	public static readonly Encoder Compression =
			new Encoder
				(new Guid("{e09d739d-ccd4-44ee-8eba-3fbf8be4fc58}"));
	public static readonly Encoder LuminanceTable =
			new Encoder
				(new Guid("{edb33bce-0266-4a77-b904-27216099e717}"));
	public static readonly Encoder Quality =
			new Encoder
				(new Guid("{1d5be4b5-fa4a-452d-9cdd-5db35105e7eb}"));
	public static readonly Encoder RenderMethod =
			new Encoder
				(new Guid("{6d42c53a-229a-4825-8bb7-5c99e2b9a8b8}"));
	public static readonly Encoder ScanMethod =
			new Encoder
				(new Guid("{3a4e2661-3109-4e56-8536-42c156e7dcfa}"));
	public static readonly Encoder Transformation =
			new Encoder
				(new Guid("{8d0eb2d1-a58e-4ea8-aa14-108074b7b6f9}"));
	public static readonly Encoder Version =
			new Encoder
				(new Guid("{24d18c76-814a-41a4-bf53-1c219cccf797}"));

	// Constructor.
	public Encoder(Guid guid)
			{
				this.guid = guid;
			}

	// Get the GUID of this encoder.
	public Guid Guid
			{
				get
				{
					return guid;
				}
			}

}; // class Encoder

#endif // !ECMA_COMPAT

}; // namespace System.Drawing.Imaging
