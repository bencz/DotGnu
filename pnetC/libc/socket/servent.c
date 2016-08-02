/*
 * servent.c - Services database entries.
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include <netdb.h>
#include <string.h>

/*
 * Define the builtin service database.  We cannot use "/etc/services"
 * for this because it won't exist on Windows systems.
 */
static char *no_aliases[] = {0};
static char *discard_aliases[] = {"sink", "null", 0};
static char *systat_aliases[] = {"users", 0};
static char *qotd_aliases[] = {"quote", 0};
static char *chargen_aliases[] = {"ttytst", "source", 0};
static char *smtp_aliases[] = {"mail", 0};
static char *time_aliases[] = {"timserver", 0};
static char *rlp_aliases[] = {"resource", 0};
static char *nameserver_aliases[] = {"name", 0};
static char *nicname_aliases[] = {"whois", 0};
static char *domain_aliases[] = {"nameserver", 0};
static char *http_aliases[] = {"www", "www-http", 0};
static char *kerberos_aliases[] = {"kerberos5", "krb5", 0};
static char *hostname_aliases[] = {"hostnames", 0};
static char *iso_tsap_aliases[] = {"tsap", 0};
static char *csnet_ns_aliases[] = {"cso", 0};
static char *pop2_aliases[] = {"pop-2", "postoffice", 0};
static char *pop3_aliases[] = {"pop-3", 0};
static char *sunrpc_aliases[] = {"portmapper", 0};
static char *auth_aliases[] = {"authentication", "tap", "ident", 0};
static char *nntp_aliases[] = {"readnews", "untp", 0};
static char *imap_aliases[] = {"imap2", 0};
static char *snmptrap_aliases[] = {"snmp-trap", 0};
static char *nextstep_aliases[] = {"NeXTStep", "NextStep", 0};
static char *z39_50_aliases[] = {"z3950", "wais", 0};
static char *link_aliases[] = {"ttylink", 0};
static char *ulistproc_aliases[] = {"ulistserv", 0};
static char *kpasswd_aliases[] = {"kpwd", 0};
static char *submission_aliases[] = {"msa", 0};
static char *npmp_local_aliases[] = {"dqs313_qmaster", 0};
static char *npmp_gui_aliases[] = {"dqs313_execd", 0};
static char *hmmp_ind_aliases[] = {"dqs313_intercell", 0};
static char *kerberos_iv_aliases[] = {"kerberos4", "kerberos-sec", "kdc", 0};
static char *biff_aliases[] = {"comsat", 0};
static char *who_aliases[] = {"whod", 0};
static char *shell_aliases[] = {"cmd", 0};
static char *printer_aliases[] = {"spooler", 0};
static char *utime_aliases[] = {"unixtime", 0};
static char *router_aliases[] = {"route", "routed", 0};
static char *timed_aliases[] = {"timeserver", 0};
static char *tempo_aliases[] = {"newdate", 0};
static char *courier_aliases[] = {"rpc", 0};
static char *conference_aliases[] = {"chat", 0};
static char *netnews_aliases[] = {"readnews", 0};
static char *uucp_aliases[] = {"uucpd", 0};
static char *kshell_aliases[] = {"krcmd", 0};
static char *remotefs_aliases[] = {"rfs_server", "rfs", 0};
static char *support_aliases[] = {"prmsd", "gnatsd", 0};
static char *datametrics_aliases[] = {"old-radius", 0};
static char *sa_msg_port_aliases[] = {"old-radacct", 0};
static char *radius_acct_aliases[] = {"radacct", 0};
static char *nfs_aliases[] = {"nfsd", 0};
static char *cvsup_aliases[] = {"CVSup", 0};
static char *x11_aliases[] = {"X", 0};
static char *krbupdate_aliases[] = {"kreg", 0};
static char *fsp_aliases[] = {"fspd", 0};
static char *omirr_aliases[] = {"omirrd", 0};
static char *jetdirect_aliases[] = {"laserjet", "hplj", 0};
static char *mandelspawn_aliases[] = {"mandelbrot", 0};
static struct servent service_entries[] = {
	{"tcpmux", no_aliases, 256, "tcp"},
	{"tcpmux", no_aliases, 256, "udp"},
	{"rje", no_aliases, 1280, "tcp"},
	{"rje", no_aliases, 1280, "udp"},
	{"echo", no_aliases, 1792, "tcp"},
	{"echo", no_aliases, 1792, "udp"},
	{"discard", discard_aliases, 2304, "tcp"},
	{"discard", discard_aliases, 2304, "udp"},
	{"systat", systat_aliases, 2816, "tcp"},
	{"systat", systat_aliases, 2816, "udp"},
	{"daytime", no_aliases, 3328, "tcp"},
	{"daytime", no_aliases, 3328, "udp"},
	{"qotd", qotd_aliases, 4352, "tcp"},
	{"qotd", qotd_aliases, 4352, "udp"},
	{"msp", no_aliases, 4608, "tcp"},
	{"msp", no_aliases, 4608, "udp"},
	{"chargen", chargen_aliases, 4864, "tcp"},
	{"chargen", chargen_aliases, 4864, "udp"},
	{"ftp-data", no_aliases, 5120, "tcp"},
	{"ftp-data", no_aliases, 5120, "udp"},
	{"ftp", no_aliases, 5376, "tcp"},
	{"ftp", no_aliases, 5376, "udp"},
	{"ssh", no_aliases, 5632, "tcp"},
	{"ssh", no_aliases, 5632, "udp"},
	{"telnet", no_aliases, 5888, "tcp"},
	{"telnet", no_aliases, 5888, "udp"},
	{"smtp", smtp_aliases, 6400, "tcp"},
	{"smtp", smtp_aliases, 6400, "udp"},
	{"time", time_aliases, 9472, "tcp"},
	{"time", time_aliases, 9472, "udp"},
	{"rlp", rlp_aliases, 9984, "tcp"},
	{"rlp", rlp_aliases, 9984, "udp"},
	{"nameserver", nameserver_aliases, 10752, "tcp"},
	{"nameserver", nameserver_aliases, 10752, "udp"},
	{"nicname", nicname_aliases, 11008, "tcp"},
	{"nicname", nicname_aliases, 11008, "udp"},
	{"tacacs", no_aliases, 12544, "tcp"},
	{"tacacs", no_aliases, 12544, "udp"},
	{"re-mail-ck", no_aliases, 12800, "tcp"},
	{"re-mail-ck", no_aliases, 12800, "udp"},
	{"domain", domain_aliases, 13568, "tcp"},
	{"domain", domain_aliases, 13568, "udp"},
	{"whois++", no_aliases, 16128, "tcp"},
	{"whois++", no_aliases, 16128, "udp"},
	{"bootps", no_aliases, 17152, "tcp"},
	{"bootps", no_aliases, 17152, "udp"},
	{"bootpc", no_aliases, 17408, "tcp"},
	{"bootpc", no_aliases, 17408, "udp"},
	{"tftp", no_aliases, 17664, "tcp"},
	{"tftp", no_aliases, 17664, "udp"},
	{"gopher", no_aliases, 17920, "tcp"},
	{"gopher", no_aliases, 17920, "udp"},
	{"netrjs-1", no_aliases, 18176, "tcp"},
	{"netrjs-1", no_aliases, 18176, "udp"},
	{"netrjs-2", no_aliases, 18432, "tcp"},
	{"netrjs-2", no_aliases, 18432, "udp"},
	{"netrjs-3", no_aliases, 18688, "tcp"},
	{"netrjs-3", no_aliases, 18688, "udp"},
	{"netrjs-4", no_aliases, 18944, "tcp"},
	{"netrjs-4", no_aliases, 18944, "udp"},
	{"finger", no_aliases, 20224, "tcp"},
	{"finger", no_aliases, 20224, "udp"},
	{"http", http_aliases, 20480, "tcp"},
	{"http", http_aliases, 20480, "udp"},
	{"kerberos", kerberos_aliases, 22528, "tcp"},
	{"kerberos", kerberos_aliases, 22528, "udp"},
	{"supdup", no_aliases, 24320, "tcp"},
	{"supdup", no_aliases, 24320, "udp"},
	{"hostname", hostname_aliases, 25856, "tcp"},
	{"hostname", hostname_aliases, 25856, "udp"},
	{"iso-tsap", iso_tsap_aliases, 26112, "tcp"},
	{"csnet-ns", csnet_ns_aliases, 26880, "tcp"},
	{"csnet-ns", csnet_ns_aliases, 26880, "udp"},
	{"rtelnet", no_aliases, 27392, "tcp"},
	{"rtelnet", no_aliases, 27392, "udp"},
	{"pop2", pop2_aliases, 27904, "tcp"},
	{"pop2", pop2_aliases, 27904, "udp"},
	{"pop3", pop3_aliases, 28160, "tcp"},
	{"pop3", pop3_aliases, 28160, "udp"},
	{"sunrpc", sunrpc_aliases, 28416, "tcp"},
	{"sunrpc", sunrpc_aliases, 28416, "udp"},
	{"auth", auth_aliases, 28928, "tcp"},
	{"auth", auth_aliases, 28928, "udp"},
	{"sftp", no_aliases, 29440, "tcp"},
	{"sftp", no_aliases, 29440, "udp"},
	{"uucp-path", no_aliases, 29952, "tcp"},
	{"uucp-path", no_aliases, 29952, "udp"},
	{"nntp", nntp_aliases, 30464, "tcp"},
	{"nntp", nntp_aliases, 30464, "udp"},
	{"ntp", no_aliases, 31488, "tcp"},
	{"ntp", no_aliases, 31488, "udp"},
	{"netbios-ns", no_aliases, 35072, "tcp"},
	{"netbios-ns", no_aliases, 35072, "udp"},
	{"netbios-dgm", no_aliases, 35328, "tcp"},
	{"netbios-dgm", no_aliases, 35328, "udp"},
	{"netbios-ssn", no_aliases, 35584, "tcp"},
	{"netbios-ssn", no_aliases, 35584, "udp"},
	{"imap", imap_aliases, 36608, "tcp"},
	{"imap", imap_aliases, 36608, "udp"},
	{"snmp", no_aliases, 41216, "tcp"},
	{"snmp", no_aliases, 41216, "udp"},
	{"snmptrap", snmptrap_aliases, 41472, "udp"},
	{"cmip-man", no_aliases, 41728, "tcp"},
	{"cmip-man", no_aliases, 41728, "udp"},
	{"cmip-agent", no_aliases, 41984, "tcp"},
	{"smip-agent", no_aliases, 41984, "udp"},
	{"mailq", no_aliases, 44544, "tcp"},
	{"mailq", no_aliases, 44544, "udp"},
	{"xdmcp", no_aliases, 45312, "tcp"},
	{"xdmcp", no_aliases, 45312, "udp"},
	{"nextstep", nextstep_aliases, 45568, "tcp"},
	{"nextstep", nextstep_aliases, 45568, "udp"},
	{"bgp", no_aliases, 45824, "tcp"},
	{"bgp", no_aliases, 45824, "udp"},
	{"prospero", no_aliases, 48896, "tcp"},
	{"prospero", no_aliases, 48896, "udp"},
	{"irc", no_aliases, 49664, "tcp"},
	{"irc", no_aliases, 49664, "udp"},
	{"smux", no_aliases, 50944, "tcp"},
	{"smux", no_aliases, 50944, "udp"},
	{"at-rtmp", no_aliases, 51456, "tcp"},
	{"at-rtmp", no_aliases, 51456, "udp"},
	{"at-nbp", no_aliases, 51712, "tcp"},
	{"at-nbp", no_aliases, 51712, "udp"},
	{"at-echo", no_aliases, 52224, "tcp"},
	{"at-echo", no_aliases, 52224, "udp"},
	{"at-zis", no_aliases, 52736, "tcp"},
	{"at-zis", no_aliases, 52736, "udp"},
	{"qmtp", no_aliases, 53504, "tcp"},
	{"qmtp", no_aliases, 53504, "udp"},
	{"z39.50", z39_50_aliases, 53760, "tcp"},
	{"z39.50", z39_50_aliases, 53760, "udp"},
	{"ipx", no_aliases, 54528, "tcp"},
	{"ipx", no_aliases, 54528, "udp"},
	{"imap3", no_aliases, 56320, "tcp"},
	{"imap3", no_aliases, 56320, "udp"},
	{"link", link_aliases, 62720, "tcp"},
	{"link", link_aliases, 62720, "ucp"},
	{"rsvp_tunnel", no_aliases, 27393, "tcp"},
	{"rsvp_tunnel", no_aliases, 27393, "udp"},
	{"rpc2portmap", no_aliases, 28929, "tcp"},
	{"rpc2portmap", no_aliases, 28929, "udp"},
	{"codaauth2", no_aliases, 29185, "tcp"},
	{"codaauth2", no_aliases, 29185, "udp"},
	{"ulistproc", ulistproc_aliases, 29697, "tcp"},
	{"ulistproc", ulistproc_aliases, 29697, "udp"},
	{"ldap", no_aliases, 34049, "tcp"},
	{"ldap", no_aliases, 34049, "udp"},
	{"svrloc", no_aliases, 43777, "tcp"},
	{"svrloc", no_aliases, 43777, "udp"},
	{"mobileip-agent", no_aliases, 45569, "tcp"},
	{"mobileip-agent", no_aliases, 45569, "udp"},
	{"mobilip-mn", no_aliases, 45825, "tcp"},
	{"mobilip-mn", no_aliases, 45825, "udp"},
	{"https", no_aliases, 47873, "tcp"},
	{"https", no_aliases, 47873, "udp"},
	{"snpp", no_aliases, 48129, "tcp"},
	{"snpp", no_aliases, 48129, "udp"},
	{"microsoft-ds", no_aliases, 48385, "tcp"},
	{"microsoft-ds", no_aliases, 48385, "udp"},
	{"kpasswd", kpasswd_aliases, 53249, "tcp"},
	{"kpasswd", kpasswd_aliases, 53249, "udp"},
	{"photuris", no_aliases, 54273, "tcp"},
	{"photuris", no_aliases, 54273, "udp"},
	{"saft", no_aliases, 59137, "tcp"},
	{"saft", no_aliases, 59137, "udp"},
	{"gss-http", no_aliases, 59393, "tcp"},
	{"gss-http", no_aliases, 59393, "udp"},
	{"pim-rp-disc", no_aliases, 61441, "tcp"},
	{"pim-rp-disc", no_aliases, 61441, "udp"},
	{"isakmp", no_aliases, 62465, "tcp"},
	{"isakmp", no_aliases, 62465, "udp"},
	{"gdomap", no_aliases, 6658, "tcp"},
	{"gdomap", no_aliases, 6658, "udp"},
	{"iiop", no_aliases, 5890, "tcp"},
	{"iiop", no_aliases, 5890, "udp"},
	{"dhcpv6-client", no_aliases, 8706, "tcp"},
	{"dhcpv6-client", no_aliases, 8706, "udp"},
	{"dhcpv6-server", no_aliases, 8962, "tcp"},
	{"dhcpv6-server", no_aliases, 8962, "udp"},
	{"rtsp", no_aliases, 10754, "tcp"},
	{"rtsp", no_aliases, 10754, "udp"},
	{"nntps", no_aliases, 13058, "tcp"},
	{"nntps", no_aliases, 13058, "udp"},
	{"whoami", no_aliases, 13570, "tcp"},
	{"whoami", no_aliases, 13570, "udp"},
	{"submission", submission_aliases, 19202, "tcp"},
	{"submission", submission_aliases, 19202, "udp"},
	{"npmp-local", npmp_local_aliases, 25090, "tcp"},
	{"npmp-local", npmp_local_aliases, 25090, "udp"},
	{"npmp-gui", npmp_gui_aliases, 25346, "tcp"},
	{"npmp-gui", npmp_gui_aliases, 25346, "udp"},
	{"hmmp-ind", hmmp_ind_aliases, 25602, "tcp"},
	{"hmmp-ind", hmmp_ind_aliases, 25602, "udp"},
	{"ldaps", no_aliases, 31746, "tcp"},
	{"ldaps", no_aliases, 31746, "udp"},
	{"acap", no_aliases, 41474, "tcp"},
	{"acap", no_aliases, 41474, "udp"},
	{"ha-cluster", no_aliases, 46594, "tcp"},
	{"ha-cluster", no_aliases, 46594, "udp"},
	{"kerberos-adm", no_aliases, 60674, "tcp"},
	{"kerberos-iv", kerberos_iv_aliases, 60930, "udp"},
	{"kerberos-iv", kerberos_iv_aliases, 60930, "tcp"},
	{"webster", no_aliases, 64770, "tcp"},
	{"webster", no_aliases, 64770, "udp"},
	{"phonebook", no_aliases, 65282, "tcp"},
	{"phonebook", no_aliases, 65282, "udp"},
	{"rsync", no_aliases, 26883, "tcp"},
	{"rsync", no_aliases, 26883, "udp"},
	{"telnets", no_aliases, 57347, "tcp"},
	{"telnets", no_aliases, 57347, "udp"},
	{"imaps", no_aliases, 57603, "tcp"},
	{"imaps", no_aliases, 57603, "udp"},
	{"ircs", no_aliases, 57859, "tcp"},
	{"ircs", no_aliases, 57859, "udp"},
	{"pop3s", no_aliases, 58115, "tcp"},
	{"pop3s", no_aliases, 58115, "udp"},
	{"exec", no_aliases, 2, "tcp"},
	{"biff", biff_aliases, 2, "udp"},
	{"login", no_aliases, 258, "tcp"},
	{"who", who_aliases, 258, "udp"},
	{"shell", shell_aliases, 514, "tcp"},
	{"syslog", no_aliases, 514, "udp"},
	{"printer", printer_aliases, 770, "tcp"},
	{"printer", printer_aliases, 770, "udp"},
	{"talk", no_aliases, 1282, "udp"},
	{"ntalk", no_aliases, 1538, "udp"},
	{"utime", utime_aliases, 1794, "tcp"},
	{"utime", utime_aliases, 1794, "udp"},
	{"efs", no_aliases, 2050, "tcp"},
	{"router", router_aliases, 2050, "udp"},
	{"ripng", no_aliases, 2306, "tcp"},
	{"ripng", no_aliases, 2306, "udp"},
	{"timed", timed_aliases, 3330, "tcp"},
	{"timed", timed_aliases, 3330, "udp"},
	{"tempo", tempo_aliases, 3586, "tcp"},
	{"courier", courier_aliases, 4610, "tcp"},
	{"conference", conference_aliases, 4866, "tcp"},
	{"netnews", netnews_aliases, 5122, "tcp"},
	{"netwall", no_aliases, 5378, "udp"},
	{"uucp", uucp_aliases, 7170, "tcp"},
	{"klogin", no_aliases, 7938, "tcp"},
	{"kshell", kshell_aliases, 8194, "tcp"},
	{"afpovertcp", no_aliases, 9218, "tcp"},
	{"afpovertcp", no_aliases, 9218, "udp"},
	{"remotefs", remotefs_aliases, 11266, "tcp"},
	{"socks", no_aliases, 14340, "tcp"},
	{"socks", no_aliases, 14340, "udp"},
	{"skkserv", no_aliases, 39428, "tcp"},
	{"h323hostcallsc", no_aliases, 5125, "tcp"},
	{"h323hostcallsc", no_aliases, 5125, "udp"},
	{"ms-sql-s", no_aliases, 39173, "tcp"},
	{"ms-sql-s", no_aliases, 39173, "udp"},
	{"ms-sql-m", no_aliases, 39429, "tcp"},
	{"ms-sql-m", no_aliases, 39429, "udp"},
	{"ica", no_aliases, 54789, "tcp"},
	{"ica", no_aliases, 54789, "udp"},
	{"wins", no_aliases, 59397, "tcp"},
	{"wins", no_aliases, 59397, "udp"},
	{"ingreslock", no_aliases, 62469, "tcp"},
	{"ingreslock", no_aliases, 62469, "udp"},
	{"prospero-np", no_aliases, 62725, "tcp"},
	{"prospero-np", no_aliases, 62725, "udp"},
	{"support", support_aliases, 63749, "tcp"},
	{"datametrics", datametrics_aliases, 27910, "tcp"},
	{"datametrics", datametrics_aliases, 27910, "udp"},
	{"sa-msg-port", sa_msg_port_aliases, 28166, "tcp"},
	{"sa-msg-port", sa_msg_port_aliases, 28166, "udp"},
	{"kermit", no_aliases, 28934, "tcp"},
	{"kermit", no_aliases, 28934, "udp"},
	{"l2tp", no_aliases, 42246, "tcp"},
	{"l2tp", no_aliases, 42246, "udp"},
	{"h323gatedisc", no_aliases, 46598, "tcp"},
	{"h323gatedisc", no_aliases, 46598, "udp"},
	{"h323gatestat", no_aliases, 46854, "tcp"},
	{"h323gatestat", no_aliases, 46854, "udp"},
	{"h323hostcall", no_aliases, 47110, "tcp"},
	{"h323hostcall", no_aliases, 47110, "udp"},
	{"tftp-mcast", no_aliases, 56838, "tcp"},
	{"tftp-mcast", no_aliases, 56838, "udp"},
	{"hello", no_aliases, 64518, "tcp"},
	{"hello", no_aliases, 64518, "udp"},
	{"radius", no_aliases, 5127, "tcp"},
	{"radius", no_aliases, 5127, "udp"},
	{"radius-acct", radius_acct_aliases, 5383, "tcp"},
	{"radius-acct", radius_acct_aliases, 5383, "udp"},
	{"mtp", no_aliases, 30471, "tcp"},
	{"mtp", no_aliases, 30471, "udp"},
	{"hsrp", no_aliases, 49415, "tcp"},
	{"hsrp", no_aliases, 49415, "udp"},
	{"licensedaemon", no_aliases, 49671, "tcp"},
	{"licensedaemon", no_aliases, 49671, "udp"},
	{"gdp-port", no_aliases, 52487, "tcp"},
	{"gdp-port", no_aliases, 52487, "udp"},
	{"nfs", nfs_aliases, 264, "tcp"},
	{"nfs", nfs_aliases, 264, "udp"},
	{"zephyr-srv", no_aliases, 13832, "tcp"},
	{"zephyr-srv", no_aliases, 13832, "udp"},
	{"zephyr-clt", no_aliases, 14088, "tcp"},
	{"zephyr-clt", no_aliases, 14088, "udp"},
	{"zephyr-hm", no_aliases, 14344, "tcp"},
	{"zephyr-hm", no_aliases, 14344, "udp"},
	{"cvspserver", no_aliases, 24841, "tcp"},
	{"cvspserver", no_aliases, 24841, "udp"},
	{"venus", no_aliases, 32265, "tcp"},
	{"venus", no_aliases, 32265, "udp"},
	{"venus-se", no_aliases, 32521, "tcp"},
	{"venus-se", no_aliases, 32521, "udp"},
	{"codasrv", no_aliases, 32777, "tcp"},
	{"codasrv", no_aliases, 32777, "udp"},
	{"codasrv-se", no_aliases, 33033, "tcp"},
	{"codasrv-se", no_aliases, 33033, "udp"},
	{"corbaloc", no_aliases, 63754, "tcp"},
	{"icpv2", no_aliases, 14860, "tcp"},
	{"icpv2", no_aliases, 14860, "udp"},
	{"mysql", no_aliases, 59916, "tcp"},
	{"mysql", no_aliases, 59916, "udp"},
	{"trnsprntproxy", no_aliases, 4621, "tcp"},
	{"trnsprntproxy", no_aliases, 4621, "udp"},
	{"prsvp", no_aliases, 32525, "tcp"},
	{"prsvp", no_aliases, 32525, "udp"},
	{"rwhois", no_aliases, 57616, "tcp"},
	{"rwhois", no_aliases, 57616, "udp"},
	{"krb524", no_aliases, 23569, "tcp"},
	{"krb524", no_aliases, 23569, "udp"},
	{"rfe", no_aliases, 35347, "tcp"},
	{"rfe", no_aliases, 35347, "udp"},
	{"cfengine", no_aliases, 48148, "tcp"},
	{"cfengine", no_aliases, 48148, "udp"},
	{"cvsup", cvsup_aliases, 28439, "tcp"},
	{"cvsup", cvsup_aliases, 28439, "udp"},
	{"x11", x11_aliases, 28695, "tcp"},
	{"afs3-fileserver", no_aliases, 22555, "tcp"},
	{"afs3-fileserver", no_aliases, 22555, "udp"},
	{"afs3-callback", no_aliases, 22811, "tcp"},
	{"afs3-callback", no_aliases, 22811, "udp"},
	{"afs3-prserver", no_aliases, 23067, "tcp"},
	{"afs3-prserver", no_aliases, 23067, "udp"},
	{"afs3-vlserver", no_aliases, 23323, "tcp"},
	{"afs3-vlserver", no_aliases, 23323, "udp"},
	{"afs3-kaserver", no_aliases, 23579, "tcp"},
	{"afs3-kaserver", no_aliases, 23579, "udp"},
	{"afs3-volser", no_aliases, 23835, "tcp"},
	{"afs3-volser", no_aliases, 23835, "udp"},
	{"afs3-errors", no_aliases, 24091, "tcp"},
	{"afs3-errors", no_aliases, 24091, "udp"},
	{"afs3-bos", no_aliases, 24347, "tcp"},
	{"afs3-bos", no_aliases, 24347, "udp"},
	{"afs3-update", no_aliases, 24603, "tcp"},
	{"afs3-update", no_aliases, 24603, "udp"},
	{"afs3-rmtsys", no_aliases, 24859, "tcp"},
	{"afs3-rmtsys", no_aliases, 24859, "udp"},
	{"sd", no_aliases, 37926, "tcp"},
	{"sd", no_aliases, 37926, "udp"},
	{"amanda", no_aliases, 24615, "tcp"},
	{"amanda", no_aliases, 24615, "udp"},
	{"h323callsigalt", no_aliases, 51245, "tcp"},
	{"h323callsigalt", no_aliases, 51245, "udp"},
	{"quake", no_aliases, 36965, "tcp"},
	{"quake", no_aliases, 36965, "udp"},
	{"wnn6-ds", no_aliases, 24678, "tcp"},
	{"wnn6-ds", no_aliases, 24678, "udp"},
	{"traceroute", no_aliases, 39554, "tcp"},
	{"traceroute", no_aliases, 39554, "udp"},
	{"rtmp", no_aliases, 256, "ddp"},
	{"nbp", no_aliases, 512, "ddp"},
	{"echo", no_aliases, 1024, "ddp"},
	{"zip", no_aliases, 1536, "ddp"},
	{"kerberos_master", no_aliases, 61186, "udp"},
	{"kerberos_master", no_aliases, 61186, "tcp"},
	{"passwd_server", no_aliases, 61442, "udp"},
	{"krbupdate", krbupdate_aliases, 63490, "tcp"},
	{"kpop", no_aliases, 21764, "tcp"},
	{"knetd", no_aliases, 1288, "tcp"},
	{"krb5_prop", no_aliases, 61954, "tcp"},
	{"eklogin", no_aliases, 14600, "tcp"},
	{"supfilesrv", no_aliases, 26371, "tcp"},
	{"supfiledbg", no_aliases, 26372, "tcp"},
	{"netstat", no_aliases, 3840, "tcp"},
	{"fsp", fsp_aliases, 5376, "udp"},
	{"linuxconf", no_aliases, 25088, "tcp"},
	{"poppassd", no_aliases, 27136, "tcp"},
	{"poppassd", no_aliases, 27136, "udp"},
	{"smtps", no_aliases, 53505, "tcp"},
	{"gii", no_aliases, 26626, "tcp"},
	{"omirr", omirr_aliases, 10243, "tcp"},
	{"omirr", omirr_aliases, 10243, "udp"},
	{"swat", no_aliases, 34051, "tcp"},
	{"rmtcfg", no_aliases, 54276, "tcp"},
	{"xtel", no_aliases, 8453, "tcp"},
	{"support", no_aliases, 63749, "tcp"},
	{"cfinger", no_aliases, 54023, "tcp"},
	{"ninstall", no_aliases, 26120, "tcp"},
	{"ninstall", no_aliases, 26120, "udp"},
	{"afbackup", no_aliases, 44043, "tcp"},
	{"afbackup", no_aliases, 44043, "udp"},
	{"squid", no_aliases, 14348, "tcp"},
	{"postgres", no_aliases, 14357, "tcp"},
	{"postgres", no_aliases, 14357, "udp"},
	{"fax", no_aliases, 52497, "tcp"},
	{"hylafax", no_aliases, 53009, "tcp"},
	{"sgi-dgl", no_aliases, 28692, "tcp"},
	{"sgi-dgl", no_aliases, 28692, "udp"},
	{"noclog", no_aliases, 59924, "tcp"},
	{"noclog", no_aliases, 59924, "udp"},
	{"hostmon", no_aliases, 60180, "tcp"},
	{"hostmon", no_aliases, 60180, "udp"},
	{"ircd", no_aliases, 2842, "tcp"},
	{"ircd", no_aliases, 2842, "udp"},
	{"xfs", no_aliases, 48155, "tcp"},
	{"tircproxy", no_aliases, 61981, "tcp"},
	{"http-alt", no_aliases, 18463, "tcp"},
	{"http-alt", no_aliases, 18463, "udp"},
	{"webcache", no_aliases, 36895, "tcp"},
	{"webcache", no_aliases, 36895, "udp"},
	{"tproxy", no_aliases, 37151, "tcp"},
	{"tproxy", no_aliases, 37151, "udp"},
	{"jetdirect", jetdirect_aliases, 35875, "tcp"},
	{"mandelspawn", mandelspawn_aliases, 36644, "udp"},
	{"kamanda", no_aliases, 24871, "tcp"},
	{"kamanda", no_aliases, 24871, "udp"},
	{"amandaidx", no_aliases, 25127, "tcp"},
	{"amidxtape", no_aliases, 25383, "tcp"},
	{"isdnlog", no_aliases, 11086, "tcp"},
	{"isdnlog", no_aliases, 11086, "udp"},
	{"vboxd", no_aliases, 11342, "tcp"},
	{"vboxd", no_aliases, 11342, "udp"},
	{"binkp", no_aliases, 59999, "tcp"},
	{"binkp", no_aliases, 59999, "udp"},
	{"asp", no_aliases, 61034, "tcp"},
	{"asp", no_aliases, 61034, "udp"},
	{"tfido", no_aliases, 4587, "tcp"},
	{"tfido", no_aliases, 4587, "udp"},
	{"fido", no_aliases, 5099, "tcp"},
	{"fido", no_aliases, 5099, "udp"},
	{"wnn4", no_aliases, 343, "tcp"},
};
#define	num_service_entries (sizeof(service_entries) / sizeof(struct servent))

static int __declspec(thread) servent_posn = 0;

void
setservent (int stay_open)
{
  servent_posn = 0;
}

void endservent (void)
{
  servent_posn = 0;
}

struct servent *
getservent (void)
{
  if (servent_posn >= 0 && servent_posn < num_service_entries)
    return &(service_entries[servent_posn++]);
  else
    return 0;
}

int
getservent_r (struct servent *__restrict result_buf,
	      char *__restrict buf, size_t buflen,
              struct servent **__restrict result)
{
  struct servent *ent = getservent ();
  if (ent)
    {
      memcpy (result_buf, ent, sizeof (struct servent));
      *result = result_buf;
      return 0;
    }
  else
    {
      *result = 0;
      return 0;
    }
}

int
__netdb_name_match (const char *value, char *name, char **aliases)
{
  if (!strcmp (value, name))
    return 1;
  while (*aliases != 0)
    {
      if (!strcmp (*aliases, value))
        return 1;
      ++aliases;
    }
  return 0;
}

struct servent *
getservbyname (const char *name, const char *proto)
{
  int posn;
  struct servent *ent;
  if (!name || !proto)
    return 0;
  for (posn = 0; posn < num_service_entries; ++posn)
    {
      ent = &(service_entries[posn]);
      if (__netdb_name_match (name, ent->s_name, ent->s_aliases) &&
          !strcmp (proto, ent->s_proto))
        {
	  return ent;
	}
    }
  return 0;
}

struct servent *
getservbyport (int port, const char *proto)
{
  int posn;
  struct servent *ent;
  if (!proto)
    return 0;
  for (posn = 0; posn < num_service_entries; ++posn)
    {
      ent = &(service_entries[posn]);
      if (ent->s_port == port && !strcmp (proto, ent->s_proto))
        {
	  return ent;
	}
    }
  return 0;
}

int
getservbyname_r (const char *__restrict name,
                 const char *__restrict proto,
		 struct servent *__restrict result_buf,
		 char *__restrict buf, size_t buflen,
		 struct servent **__restrict result)
{
  struct servent *ent = getservbyname (name, proto);
  if (ent)
    {
      memcpy (result_buf, ent, sizeof (struct servent));
      *result = result_buf;
      return 0;
    }
  else
    {
      *result = 0;
      return 0;
    }
}

int
getservbyport_r (int port, const char *__restrict proto,
		 struct servent *__restrict result_buf,
		 char *__restrict buf, size_t buflen,
		 struct servent **__restrict result)
{
  struct servent *ent = getservbyport (port, proto);
  if (ent)
    {
      memcpy (result_buf, ent, sizeof (struct servent));
      *result = result_buf;
      return 0;
    }
  else
    {
      *result = 0;
      return 0;
    }
}
