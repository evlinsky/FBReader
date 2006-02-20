/*
 * FBReader -- electronic book reader
 * Copyright (C) 2004-2006 Nikolay Pultsin <geometer@mawhrin.net>
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

#include <locale.h>
#include <libintl.h>
#include <gtk/gtk.h>

#include <abstract/ZLEncodingConverter.h>
#include <unix/ZLUnixFSManager.h>
#include <unix/ZLUnixTime.h>
#include <desktop/XMLOptions.h>
#include <gtk/GtkDialogManager.h>
#include <gtk/GtkImageManager.h>
#include <gtk/GtkDeviceInfo.h>

#include "GtkFBReader.h"
#include "Paths.h"

int main(int argc, char **argv) {
	/*
	HildonApp* app;
	HildonAppView* main_view;

	gtk_init( &argc, &argv );

	app = HILDON_APP ( hildon_app_new () );
	hildon_app_set_title ( app, "MaemoPad" );
	hildon_app_set_two_part_title ( app, TRUE );

	osso_initialize ( "FBReader", "0.0", TRUE, NULL );
	main_view = HILDON_APPVIEW(hildon_appview_new("MyMainView"));
	hildon_app_set_appview( app, main_view );

	gtk_widget_show(GTK_WIDGET(app));
	gtk_main();

	return 0;
	*/
	gtk_disable_setlocale();

	gtk_init(&argc, &argv);

	ZLUnixFSManager::createInstance();
	ZLUnixTimeManager::createInstance();
	ZLEncodingConverter::setEncodingDescriptionPath(EncodingDescriptionPath);
	XMLOptions::createInstance("FBReader");
	GtkDialogManager::createInstance();
	GtkImageManager::createInstance();
	((GtkDialogManager&)GtkDialogManager::instance()).setPixmapPath(GtkFBReader::ImageDirectory);
	GtkDeviceInfo::createInstance();

	GtkFBReader *reader = new GtkFBReader(argc == 1 ? std::string() : argv[1]);

	((GtkDialogManager&)GtkDialogManager::instance()).setMainWindow(GTK_WINDOW(reader->getMainWindow()));

	gtk_main();

	GtkDeviceInfo::deleteInstance();
	GtkImageManager::deleteInstance();
	GtkDialogManager::deleteInstance();
	XMLOptions::deleteInstance();
	ZLUnixFSManager::deleteInstance();
	ZLUnixTimeManager::deleteInstance();

	return 0;
}
