/*
 * IMembershipCondition.cs - Implementation of the
 *		"System.Security.Policy.IMembershipCondition" class.
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

namespace System.Security.Policy
{

#if CONFIG_POLICY_OBJECTS

public interface IMembershipCondition
	: ISecurityEncodable, ISecurityPolicyEncodable
{

	// Check to see if the evidence satisfies the membership condition.
	bool Check(Evidence evidence);

	// Make a copy of this object.
	IMembershipCondition Copy();

	// Determine if two membership conditions are equal.
	bool Equals(Object obj);

	// Convert this membership condition into a string.
	String ToString();

}; // interface IMembershipCondition

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Policy
