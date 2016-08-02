/*
 * keyword.cs - Test the handling of keywords in the lexer.
 *
 * "C# Language Specification", Draft 13, Section 9.4.3, "Keywords"
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

/* We test that keywords are being recognised by the lexer by
   using each keyword in a context for which it is invalid */
class abstract {}
class as {}
class base {}
class bool {}
class break {}
class byte {}
class case {}
class catch {}
class char {}
class checked {}
class class {}
class const {}
class continue {}
class decimal {}
class default {}
class delegate {}
class do {}
class double {}
class else {}
class enum {}
class event {}
class explicit {}
class extern {}
class false {}
class finally {}
class fixed {}
class float {}
class for {}
class foreach {}
class goto {}
class if {}
class implicit {}
class in {}
class int {}
class interface {}
class internal {}
class is {}
class lock {}
class long {}
class namespace {}
class new {}
class null {}
class object {}
class operator {}
class out {}
class override {}
class params {}
class private {}
class protected {}
class public {}
class readonly {}
class ref {}
class return {}
class sbyte {}
class sealed {}
class short {}
class sizeof {}
class stackalloc {}
class static {}
class string {}
class struct {}
class switch {}
class this {}
class throw {}
class true {}
class try {}
class typeof {}
class uint {}
class ulong {}
class unchecked {}
class unsafe {}
class ushort {}
class using {}
class virtual {}
class void {}
class volatile {}
class while {}

/* Special cscc keywords */
class __builtin_constant {}

/* Keywords that are used as identifiers in most situations */
class get {}
class set {}
class add {}
class remove {}
