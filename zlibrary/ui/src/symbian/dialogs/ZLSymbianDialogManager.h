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

#ifndef __ZLSYMBIANDIALOGMANAGER_H__
#define __ZLSYMBIANDIALOGMANAGER_H__

//#include <gtk/gtkwindow.h>
//#include <gtk/gtkdialog.h>

#include <ZLDialogManager.h>

class ZLSymbianDialogManager : public ZLDialogManager {

public:
	static void createInstance() { ourInstance = new ZLSymbianDialogManager(); }

private:
	ZLSymbianDialogManager() : myIsInitialized(false), /*myWindow(0),*/ myIsKeyboardGrabbed(false), myIsWaiting(false) {}

public:
	void createApplicationWindow(ZLApplication *application) const;

	shared_ptr<ZLDialog> createDialog(const ZLResourceKey &key) const;
	shared_ptr<ZLOptionsDialog> createOptionsDialog(const ZLResourceKey &key, shared_ptr<ZLRunnable> applyAction, bool showApplyButton) const;
	shared_ptr<ZLOpenFileDialog> createOpenFileDialog(const ZLResourceKey &key, const std::string &directoryPath, const std::string &filePath, const ZLOpenFileDialog::Filter &filter) const;
	void informationBox(const std::string &title, const std::string &message) const;
	void errorBox(const ZLResourceKey &key, const std::string &message) const;
	int questionBox(const ZLResourceKey &key, const std::string &message, const ZLResourceKey &button0, const ZLResourceKey &button1, const ZLResourceKey &button2) const;

	void wait(const ZLResourceKey &key, ZLRunnable &runnable) const;
	bool isWaiting() const { return myIsWaiting; }

	shared_ptr<ZLProgressDialog> createProgressDialog(const ZLResourceKey &key) const;

	bool isClipboardSupported(ClipboardType type) const;
	void setClipboardText(const std::string &text, ClipboardType type) const;
	void setClipboardImage(const ZLImageData &imageData, ClipboardType type) const;
/*
	void setMainWindow(GtkWindow *window) const { myIsInitialized = true; myWindow = window; }
*/
	void grabKeyboard(bool grab) { myIsKeyboardGrabbed = grab; }
	bool isKeyboardGrabbed() const { return myIsKeyboardGrabbed; }

private:
	mutable bool myIsInitialized;
//	mutable GtkWindow *myWindow;
	bool myIsKeyboardGrabbed;

	volatile mutable bool myIsWaiting;
/*
friend GtkDialog *createGtkDialog(const std::string &caption);
*/
friend class ZLSymbianNetworkManager;
};

#endif /* __ZLSYMBIANDIALOGMANAGER_H__ */
