/*
 * FBReader -- electronic book reader
 * Copyright (C) 2004-2007 Nikolay Pultsin <geometer@mawhrin.net>
 * Copyright (C) 2005 Mikhail Sobolev <mss@mawhrin.net>
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

#include <ZLFile.h>
#include <ZLInputStream.h>

#include "PdbPlugin.h"
#include "ZTXTStream.h"

bool ZTXTPlugin::acceptsFile(const ZLFile &file) const {
	return PdbPlugin::fileType(file) == "zTXTGPlm";
}

shared_ptr<ZLInputStream> ZTXTPlugin::createStream(ZLFile &file) const {
	return new ZTXTStream(file);
}

const std::string &ZTXTPlugin::iconName() const {
	static const std::string ICON_NAME = "weasel";
	return ICON_NAME;
}