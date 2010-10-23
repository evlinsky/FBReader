/*
 * Copyright (C) 2009-2010 Geometer Plus <contact@geometerplus.com>
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA
 * 02110-1301, USA.
 */

//#include <gtk/gtk.h>

#include <ZLStringUtil.h>

#include "ZLSymbianNetworkManager.h"
#include "../dialogs/ZLSymbianDialogManager.h"

void ZLSymbianNetworkManager::createInstance() {
//	ourInstance = new ZLMaemoNetworkManager();
}
/*
static void connectionEventHandler(ConIcConnection*, ConIcConnectionEvent *event, ZLMaemoNetworkManager *connectionManager) {
	connectionManager->onStatusChanged(con_ic_connection_event_get_status(event));
}
*//*
void ZLSymbianNetworkManager::onStatusChanged(ConIcConnectionStatus status) {
	switch(status) {
		case CON_IC_STATUS_CONNECTED:
			myConnectionStatus = CONNECTED;
			break;
		case CON_IC_STATUS_DISCONNECTING:
		case CON_IC_STATUS_DISCONNECTED:
			myConnectionStatus = FAILED;
			break;
	}
}
*/
ZLSymbianNetworkManager::ZLSymbianNetworkManager() {
/*	myConnection = con_ic_connection_new();
	g_signal_connect(G_OBJECT(myConnection), "connection-event", G_CALLBACK(connectionEventHandler), this);*/
}

ZLSymbianNetworkManager::~ZLSymbianNetworkManager() {
/*	g_object_unref(myConnection);*/
}

bool ZLSymbianNetworkManager::connect() const {
/*	volatile bool &waitingFlag = ((ZLGtkDialogManager&)ZLGtkDialogManager::Instance()).myIsWaiting;
	waitingFlag = true;
	myConnectionStatus = IN_PROGRESS;
	con_ic_connection_connect(myConnection, CON_IC_CONNECT_FLAG_UNMANAGED);
	while (myConnectionStatus == IN_PROGRESS) {
		gtk_main_iteration();
	}
	waitingFlag = false;
	return myConnectionStatus == CONNECTED;*/return 0;
}

void ZLSymbianNetworkManager::release() const {
}

bool ZLSymbianNetworkManager::providesProxyInfo() const {
	return true;
}

bool ZLSymbianNetworkManager::useProxy() const {
/*	return
		con_ic_connection_get_proxy_mode(myConnection) != CON_IC_PROXY_MODE_NONE;*/return 0;
}

std::string ZLSymbianNetworkManager::proxyHost() const {
/*	const char *host = con_ic_connection_get_proxy_host(myConnection, CON_IC_PROXY_PROTOCOL_HTTP);
	return (host != 0) ? host : std::string();*/ return NULL;
}

std::string ZLSymbianNetworkManager::proxyPort() const {
/*	int port = con_ic_connection_get_proxy_port(myConnection, CON_IC_PROXY_PROTOCOL_HTTP);
	std::string portString;
	ZLStringUtil::appendNumber(portString, port);
	return portString;*/ return NULL;
}
