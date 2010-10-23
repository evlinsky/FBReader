/*
 * Copyright (C) 2004-2010 Geometer Plus <contact@geometerplus.com>
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

#ifndef __ZLSYMBIANMESSAGE_H__
#define __ZLSYMBIANMESSAGE_H__

//#include <libosso.h>
#include <ZLMessage.h>

class ZLSymbianCommunicationManager : public ZLCommunicationManager {

public:
	static void createInstance();

	shared_ptr<ZLMessageOutputChannel> createMessageOutputChannel(const std::string &protocol, const std::string &testFile);
	void addInputMessageDescription(const std::string &command, const std::string &protocol, const Data &data);

	void init();
	void shutdown();

/*	gint onMessageReceived(const gchar *interface, const gchar *method, GArray *arguments, osso_rpc_t *retval);*/

private:
	ZLSymbianCommunicationManager();

private:
//	osso_context_t *myContext;
	std::map<std::string,std::string> myOssoMethodToCommand;
	std::map<std::string,std::string> myOssoMethodToConverter;
};

class ZLSymbianRpcMessageOutputChannel : public ZLMessageOutputChannel {

public:
/*	ZLSymbianRpcMessageOutputChannel(osso_context_t *&context);*/
	shared_ptr<ZLMessageSender> createSender(const ZLCommunicationManager::Data &data);

private:
/*	osso_context_t *&myContext;*/
};

class ZLSymbianPresentMessageOutputChannel : public ZLSymbianRpcMessageOutputChannel {

public:
/*	ZLSymbianPresentMessageOutputChannel(osso_context_t *&context);*/
	shared_ptr<ZLMessageSender> createSender(const ZLCommunicationManager::Data &data);
};

class ZLSymbianRpcMessageSender : public ZLMessageSender {

private:
/*	ZLSymbianRpcMessageSender(osso_context_t *&context, const std::string &service, const std::string &command);*/
public:
	void sendStringMessage(const std::string &message);

private:
//	osso_context_t *&myContext;
	const std::string myService;
	const std::string myCommand;

friend class ZLSymbianRpcMessageOutputChannel;
};

#endif /* __ZLSYMBIANMESSAGE_H__ */
