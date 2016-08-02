/*
 * ServiceConfig.cs - Implementation of the
 *			"System.EnterpriseServices.ServiceConfig" class.
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

namespace System.EnterpriseServices
{

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(false)]
#endif
public sealed class ServiceConfig
{
	// Internal state.
	private BindingOption binding;
	private ITransaction bringYourOwnTransaction;
	private bool comTIIntrinsicsEnabled;
	private bool iisIntrinsicsEnabled;
	private InheritanceOption inheritance;
	private TransactionIsolationLevel isolationLevel;
#if !ECMA_COMPAT
	private Guid partitionId;
#endif
	private PartitionOption partitionOption;
	private String sxsDirectory;
	private String sxsName;
	private SxsOption sxsOption;
	private SynchronizationOption synchronization;
	private ThreadPoolOption threadPool;
	private String tipUrl;
	private String trackingAppName;
	private String trackingComponentName;
	private bool trackingEnabled;
	private TransactionOption transaction;
	private String transactionDescription;
	private int transactionTimeout;

	// Constructor.
	public ServiceConfig() {}

	// Get or set this object's properties.
	public BindingOption Binding
			{
				get
				{
					return binding;
				}
				set
				{
					binding = value;
				}
			}
	public ITransaction BringYourOwnTransaction
			{
				get
				{
					return bringYourOwnTransaction;
				}
				set
				{
					bringYourOwnTransaction = value;
				}
			}
	public bool COMTIIntrinsicsEnabled
			{
				get
				{
					return comTIIntrinsicsEnabled;
				}
				set
				{
					comTIIntrinsicsEnabled = value;
				}
			}
	public bool IISIntrinsicsEnabled
			{
				get
				{
					return iisIntrinsicsEnabled;
				}
				set
				{
					iisIntrinsicsEnabled = value;
				}
			}
	public InheritanceOption Inheritance
			{
				get
				{
					return inheritance;
				}
				set
				{
					inheritance = value;
				}
			}
	public TransactionIsolationLevel IsolationLevel
			{
				get
				{
					return isolationLevel;
				}
				set
				{
					isolationLevel = value;
				}
			}
#if !ECMA_COMPAT
	public Guid PartitionId
			{
				get
				{
					return partitionId;
				}
				set
				{
					partitionId = value;
				}
			}
#endif
	public PartitionOption PartitionOption
			{
				get
				{
					return partitionOption;
				}
				set
				{
					partitionOption = value;
				}
			}
	public String SxsDirectory
			{
				get
				{
					return sxsDirectory;
				}
				set
				{
					sxsDirectory = value;
				}
			}
	public String SxsName
			{
				get
				{
					return sxsName;
				}
				set
				{
					sxsName = value;
				}
			}
	public SxsOption SxsOption
			{
				get
				{
					return sxsOption;
				}
				set
				{
					sxsOption = value;
				}
			}
	public SynchronizationOption Synchronization
			{
				get
				{
					return synchronization;
				}
				set
				{
					synchronization = value;
				}
			}
	public ThreadPoolOption ThreadPool
			{
				get
				{
					return threadPool;
				}
				set
				{
					threadPool = value;
				}
			}
	public String TipUrl
			{
				get
				{
					return tipUrl;
				}
				set
				{
					tipUrl = value;
				}
			}
	public String TrackingAppName
			{
				get
				{
					return trackingAppName;
				}
				set
				{
					trackingAppName = value;
				}
			}
	public String TrackingComponentName
			{
				get
				{
					return trackingComponentName;
				}
				set
				{
					trackingComponentName = value;
				}
			}
	public bool TrackingEnabled
			{
				get
				{
					return trackingEnabled;
				}
				set
				{
					trackingEnabled = value;
				}
			}
	public TransactionOption Transaction
			{
				get
				{
					return transaction;
				}
				set
				{
					transaction = value;
				}
			}
	public String TransactionDescription
			{
				get
				{
					return transactionDescription;
				}
				set
				{
					transactionDescription = value;
				}
			}
	public int TransactionTimeout
			{
				get
				{
					return transactionTimeout;
				}
				set
				{
					transactionTimeout = value;
				}
			}

}; // class ServiceConfig

}; // namespace System.EnterpriseServices
