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

#ifndef __ZLSYMBIANAPPLICATIONWINDOW_H__
#define __ZLSYMBIANAPPLICATIONWINDOW_H__

#include <vector>
#include <map>
#include <stack>
/*
#include <gtk/gtkwidget.h>
#include <gtk/gtkwindow.h>

#if MAEMO_VERSION <= 3
	#include <hildon-widgets/hildon-program.h>
#elif MAEMO_VERSION <= 5
	#include <hildon/hildon-program.h>
#else
	#error Unknown MAEMO_VERSION
#endif
*/
#include "../../../../core/src/application/ZLApplicationWindow.h"
#include "../../../../core/src/application/ZLMenu.h"

// ---
/*
#include <aknapp.h>
#include "fbreader.hrh"

// UID for the application;
// this should correspond to the uid defined in the mmp file
const TUid KUidfbreaderApp = {_UID3};

class CZLSymbianApplication : public CAknApplication {
public:
	TUid AppDllUid() const;

protected:
	CApaDocument* CreateDocumentL();
};



#include <akndoc.h>

class CZLSymbianAppUi;
class CEikApplication;

class CZLSymbianDocument : public CAknDocument {
public:
	static CZLSymbianDocument* NewL(CEikApplication& aApp);
	static CZLSymbianDocument* NewLC(CEikApplication& aApp);
	virtual ~CZLSymbianDocument();

public:
	CEikAppUi* CreateAppUiL();

private:
	void ConstructL();
	CfbreaderDocument(CEikApplication& aApp);
};

#include <aknappui.h>

class CZLSymbianAppView;

class CZLSymbianppUi : public CAknAppUi {
public:
	void ConstructL();
	CZLSymbianAppUi();
	virtual ~CZLSymbianAppUi();

private:
	void HandleCommandL(TInt aCommand);
	void HandleStatusPaneSizeChange();
	CArrayFix<TCoeHelpContext>* HelpContextL() const;

private:
	CZLSymbianAppView* iAppView;
};

*/
class ZLSymbianViewWidget;

class ZLSymbianApplicationWindow : public ZLApplicationWindow { 

private:
	ZLBooleanOption KeyActionOnReleaseNotOnPressOption;

public:
	ZLSymbianApplicationWindow(ZLApplication *application);
	~ZLSymbianApplicationWindow();

private:
	ZLViewWidget *createViewWidget();
	void init();
//	void updatePopupData(GtkMenuToolButton *button, shared_ptr<ZLPopupData> data);
	void addToolbarItem(ZLToolbar::ItemPtr item);

	class MenuBuilder : public ZLMenuVisitor {

	public:
		MenuBuilder(ZLSymbianApplicationWindow &window);

	private:
		void processSubmenuBeforeItems(ZLMenubar::Submenu &submenu);
		void processSubmenuAfterItems(ZLMenubar::Submenu &submenu);
		void processItem(ZLMenubar::PlainItem &item);
		void processSepartor(ZLMenubar::Separator &separator);

	private:
		ZLSymbianApplicationWindow &myWindow;
	};
	void initMenu();
	void refresh();
	void processAllEvents();
	void present();
	void close();

//	void setCaption(const std::string &caption) { gtk_window_set_title(GTK_WINDOW(myWindow), caption.c_str()); }

	bool isFullscreen() const;
	void setFullscreen(bool fullscreen);

	void grabAllKeys(bool grab);

	void setToggleButtonState(const ZLToolbar::ToggleButtonItem &button);
	void setToolbarItemState(ZLToolbar::ItemPtr item, bool visible, bool enabled);

public:
//	bool handleKeyEventSlot(GdkEventKey *event, bool isKeyRelease);
//	HildonWindow *getMainWindow() const { return myWindow; }
	void setFocusToMainWidget();

private:
//	HildonProgram *myProgram;
//	HildonWindow *myWindow;
//	HildonAppMenu *myMenu;
	ZLSymbianViewWidget *myViewWidget;

	bool myFullScreen;

//	std::map<std::string,GtkWidget*> myMenuItems;
//	std::vector<GtkMenuItem*> mySubmenuItems;

friend class MenuBuilder;
};

#endif /* __ZLSYMBIANAPPLICATIONWINDOW_H__ */
