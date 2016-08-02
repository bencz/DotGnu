//
// assembly:	System
// namespace:	System.Text.RegularExpressions
// file:	RegexRunner.cs
//
// author:	Dan Lewis (dihlewis@yahoo.co.uk)
// 		(c) 2002

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;

namespace System.Text.RegularExpressions {
	[EditorBrowsable (EditorBrowsableState.Never)]
	public abstract class RegexRunner {
		// constructor

		[TODO]
		protected internal RegexRunner () {
			throw new NotImplementedException ("RegexRunner is not supported by Mono.");
		}

		// protected abstract

		protected abstract bool FindFirstChar ();

		protected abstract void Go ();

		protected abstract void InitTrackCount ();

		// protected methods

		[TODO]
		protected void Capture (int capnum, int start, int end) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected static bool CharInSet (char ch, string set, string category) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected void Crawl (int i) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected int Crawlpos () {
			throw new NotImplementedException ();
		}

		[TODO]
		protected void DoubleCrawl () {
			throw new NotImplementedException ();
		}

		[TODO]
		protected void DoubleStack () {
			throw new NotImplementedException ();
		}

		[TODO]
		protected void DoubleTrack () {
			throw new NotImplementedException ();
		}

		[TODO]
		protected void EnsureStorage () {
			throw new NotImplementedException ();
		}

		[TODO]
		protected bool IsBoundary (int index, int startpos, int endpos) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected bool IsECMABoundary (int index, int startpos, int endpos) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected bool IsMatched (int cap) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected int MatchIndex (int cap) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected int MatchLength (int cap) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected int Popcrawl () {
			throw new NotImplementedException ();
		}

		[TODO]
		protected void TransferCapture (int capnum, int uncapnum, int start, int end) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected void Uncapture () {
			throw new NotImplementedException ();
		}

		// internal
		
		protected internal Match Scan (Regex regex, string text, int textbeg, int textend, int textstart, int prevlen, bool quick) {
			throw new NotImplementedException ();
		}

		[TODO]
		protected internal int[] runcrawl;
		[TODO]
		protected internal int runcrawlpos;
		[TODO]
		protected internal Match runmatch;
		[TODO]
		protected internal Regex runregex;
		[TODO]
		protected internal int[] runstack;
		[TODO]
		protected internal int runstackpos;
		[TODO]
		protected internal string runtext;
		[TODO]
		protected internal int runtextbeg;
		[TODO]
		protected internal int runtextend;
		[TODO]
		protected internal int runtextpos;
		[TODO]
		protected internal int runtextstart;
		[TODO]
		protected internal int[] runtrack;
		[TODO]
		protected internal int runtrackcount;
		[TODO]
		protected internal int runtrackpos;
	}
}
