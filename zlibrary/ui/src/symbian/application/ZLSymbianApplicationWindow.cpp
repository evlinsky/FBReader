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

//#include <gtk/gtk.h>
//#include <gdk/gdkkeysyms.h>

#include <ZLibrary.h>
#include <ZLOptionEntry.h>
#include <optionEntries/ZLSimpleOptionEntry.h>
#include <ZLPopupData.h>

#include "../message/ZLSymbianMessage.h"
#include "../view/ZLSymbianViewWidget.h"
//#include "../../gtk/util/ZLGtkKeyUtil.h"
//#include "../../gtk/util/ZLGtkSignalUtil.h"
#include "../dialogs/ZLSymbianDialogManager.h"
#include "../dialogs/ZLSymbianOptionsDialog.h"
#include "../optionView/ZLSymbianOptionViewHolder.h"
#include "../../../../core/src/dialogs/ZLOptionView.h"

#include "ZLSymbianApplicationWindow.h"

/*
#include <avkon.hrh>
#include <aknmessagequerydialog.h>
#include <aknnotewrappers.h>
#include <stringloader.h>
#include <f32file.h>
#include <s32file.h>
#include <hlplch.h>

#include <fbreader_0xE285CF01.rsg>

#include "fbreader_0xE285CF01.hlp.hrh"
#include "fbreader.hrh"
#include "fbreader.pan"



CApaDocument* CZLSymbianApplication::CreateDocumentL() {
	return CZLSymbianDocument::NewL(*this);
}

TUid CZLSymbianApplication::AppDllUid() const {
	// Return the UID for the fbreader application
	return KUidfbreaderApp;
}

// ---

CZLSymbianDocument* CZLSymbianDocument::NewL(CEikApplication& aApp) {
	CZLSymbianDocument* self = NewLC(aApp);
	CleanupStack::Pop(self);
	return self;
}

CZLSymbianDocument* CZLSymbianDocument::NewLC(CEikApplication& aApp) {
	CZLSymbianDocument* self = new (ELeave) CZLSymbianDocument(aApp);

	CleanupStack::PushL(self);
	self->ConstructL();
	return self;
}

void CZLSymbianDocument::ConstructL() {
	// No implementation required
}

CZLSymbianDocument::CZLSymbianDocument(CEikApplication& aApp) : CAknDocument(aApp) {
	// No implementation required
}

CZLSymbianDocument::~CZLSymbianDocument() {
	// No implementation required
}

CEikAppUi* CZLSymbianDocument::CreateAppUiL() {
	// Create the application user interface, and return a pointer to it;
	// the framework takes ownership of this object
	return new (ELeave) CfbreaderAppUi;
}

// ---


_LIT( KFileName, "C:\\private\\E285CF01\\fbreader.txt" );
_LIT( KText, "Hello World!");

void CZLSymbianAppUi::ConstructL() {
	// Initialise app UI with standard value.
	BaseConstructL(CAknAppUi::EAknEnableSkin);

	// Create view object
	iAppView = CZLSymbianAppView::NewL(ClientRect());

	// Create a file to write the text to
	TInt err = CCoeEnv::Static()->FsSession().MkDirAll(KFileName);
	if ((KErrNone != err) && (KErrAlreadyExists != err)) {
		return;
	}

	RFile file;
	err = file.Replace(CCoeEnv::Static()->FsSession(), KFileName, EFileWrite);
	CleanupClosePushL(file);
	if (KErrNone != err) {
		CleanupStack::PopAndDestroy(1); // file
		return;
	}

	RFileWriteStream outputFileStream(file);
	CleanupClosePushL(outputFileStream);
	outputFileStream << KText;

	CleanupStack::PopAndDestroy(2); // outputFileStream, file

}

CfbreaderAppUi::CZLSymbianAppUi() {
	// No implementation required
}

CZLSymbianAppUi::~CZLSymbianAppUi() {
	if (iAppView) {
		delete iAppView;
		iAppView = NULL;
	}

}

void CZLSymbianAppUi::HandleCommandL(TInt aCommand) {
	switch (aCommand) {
		case EEikCmdExit:
		case EAknSoftkeyExit:
			Exit();
			break;

		case ECommand1: {

			// Load a string from the resource file and display it
			HBufC* textResource = StringLoader::LoadLC(R_COMMAND1_TEXT);
			CAknInformationNote* informationNote;

			informationNote = new (ELeave) CAknInformationNote;

			// Show the information Note with
			// textResource loaded with StringLoader.
			informationNote->ExecuteLD(*textResource);

			// Pop HBuf from CleanUpStack and Destroy it.
			CleanupStack::PopAndDestroy(textResource);
		}
		break;
		
		case ECommand2: {
			RFile rFile;

			//Open file where the stream text is
			User::LeaveIfError(rFile.Open(CCoeEnv::Static()->FsSession(),
					KFileName, EFileStreamText));//EFileShareReadersOnly));// EFileStreamText));
			CleanupClosePushL(rFile);

			// copy stream from file to RFileStream object
			RFileReadStream inputFileStream(rFile);
			CleanupClosePushL(inputFileStream);

			// HBufC descriptor is created from the RFileStream object.
			HBufC* fileData = HBufC::NewLC(inputFileStream, 32);

			CAknInformationNote* informationNote;

			informationNote = new (ELeave) CAknInformationNote;
			// Show the information Note
			informationNote->ExecuteLD(*fileData);

			// Pop loaded resources from the cleanup stack
			CleanupStack::PopAndDestroy(3); // filedata, inputFileStream, rFile
		}
		break;

		case EHelp: {

			CArrayFix<TCoeHelpContext>* buf = CCoeAppUi::AppHelpContextL();
			HlpLauncher::LaunchHelpApplicationL(iEikonEnv->WsSession(), buf);
		}
		break;

		case EAbout: {

			CAknMessageQueryDialog* dlg = new (ELeave) CAknMessageQueryDialog();
			dlg->PrepareLC(R_ABOUT_QUERY_DIALOG);
			HBufC* title = iEikonEnv->AllocReadResourceLC(R_ABOUT_DIALOG_TITLE);
			dlg->QueryHeading()->SetTextL(*title);
			CleanupStack::PopAndDestroy(); //title
			HBufC* msg = iEikonEnv->AllocReadResourceLC(R_ABOUT_DIALOG_TEXT);
			dlg->SetMessageTextL(*msg);
			CleanupStack::PopAndDestroy(); //msg
			dlg->RunLD();
		}
		break;
		
		default:
			Panic( EZLSymbianUi);
		break;
		
	}
}

// -----------------------------------------------------------------------------
//  Called by the framework when the application status pane
//  size is changed.  Passes the new client rectangle to the
//  AppView
// -----------------------------------------------------------------------------
//
void CZLSymbianAppUi::HandleStatusPaneSizeChange() {
	iAppView->SetRect(ClientRect());
}

CArrayFix<TCoeHelpContext>* CZLSymbianAppUi::HelpContextL() const {
#warning "Please see comment about help and UID3..."
	// Note: Help will not work if the application uid3 is not in the
	// protected range.  The default uid3 range for projects created
	// from this template (0xE0000000 - 0xEFFFFFFF) are not in the protected range so that they
	// can be self signed and installed on the device during testing.
	// Once you get your official uid3 from Symbian Ltd. and find/replace
	// all occurrences of uid3 in your project, the context help will
	// work. Alternatively, a patch now exists for the versions of 
	// HTML help compiler in SDKs and can be found here along with an FAQ:
	// http://www3.symbian.com/faq.nsf/AllByDate/E9DF3257FD565A658025733900805EA2?OpenDocument
	CArrayFixFlat<TCoeHelpContext>* array = new (ELeave) CArrayFixFlat<
			TCoeHelpContext> (1);
	CleanupStack::PushL(array);
	array->AppendL(TCoeHelpContext(KUidZLSymbianApp, KGeneral_Information));
	CleanupStack::Pop(array);
	return array;
}

*/
// ---

void ZLSymbianDialogManager::createApplicationWindow(ZLApplication *application) const {
/*	myWindow = GTK_WINDOW((new ZLGtkApplicationWindow(application))->getMainWindow());
	myIsInitialized = true;*/
}

static bool acceptAction() {
/*	return
		ZLSymbianDialogManager::isInitialized() &&
		!((ZLSymbianDialogManager&)ZLSymbianDialogManager::Instance()).isWaiting();*/return true;
}
/*
static bool applicationQuit(GtkWidget*, GdkEvent*, gpointer data) {
	if (acceptAction()) {
		((ZLSymbianApplicationWindow*)data)->application().closeView();
	}
	return true;
}
*//*
static void repaint(GtkWidget*, GdkEvent*, gpointer data) {
	if (acceptAction()) {
		((ZLSymbianViewWidget*)data)->doPaint();
	}
}
*//*
static void menuActionSlot(GtkWidget*, gpointer data) {
	if (acceptAction()) {
		((ZLApplication::Action*)data)->checkAndRun();
	}
}
*//*
static bool handleKeyPress(GtkWidget*, GdkEventKey *key, gpointer data) {
	if (acceptAction()) {
		return ((ZLSymbianApplicationWindow*)data)->handleKeyEventSlot(key, false);
	}
	return true;
}
*//*
static bool handleKeyRelease(GtkWidget*, GdkEventKey *key, gpointer data) {
	if (acceptAction()) {
		return ((ZLSymbianApplicationWindow*)data)->handleKeyEventSlot(key, true);
	}
	return true;
}
*//*
static void mousePressed(GtkWidget *area, GdkEventButton *event, gpointer data) {
	gtk_window_set_focus(GTK_WINDOW(gtk_widget_get_toplevel(area)), area);
	if (acceptAction()) {
		((ZLSymbianViewWidget*)data)->onMousePressed(event);
	}
}
*//*
static void mouseReleased(GtkWidget*, GdkEventButton *event, gpointer data) {
	if (acceptAction()) {
		((ZLSymbianViewWidget*)data)->onMouseReleased(event);
	}
}
*//*
static void mouseMoved(GtkWidget*, GdkEventMotion *event, gpointer data) {
	if (acceptAction()) {
		((ZLSymbianViewWidget*)data)->onMouseMoved(event);
	}
}
*/
ZLSymbianApplicationWindow::ZLSymbianApplicationWindow(ZLApplication *application) :
	ZLApplicationWindow(application),
	KeyActionOnReleaseNotOnPressOption(ZLCategoryKey::CONFIG, "KeyAction", "OnRelease", false),
	myFullScreen(false) {
/*	myProgram = HILDON_PROGRAM(hildon_program_get_instance());
	g_set_application_name("");

	myWindow = HILDON_WINDOW(hildon_stackable_window_new());

	((ZLMaemoCommunicationManager&)ZLCommunicationManager::Instance()).init();

	myMenu = HILDON_APP_MENU(hildon_app_menu_new());
	hildon_window_set_app_menu(myWindow, myMenu);

	hildon_program_add_window(myProgram, myWindow);
	gtk_widget_show_all(GTK_WIDGET(myWindow));

	myViewWidget = 0;

	ZLGtkSignalUtil::connectSignal(GTK_OBJECT(myWindow), "delete_event", GTK_SIGNAL_FUNC(applicationQuit), this);
	ZLGtkSignalUtil::connectSignal(GTK_OBJECT(myWindow), "key_press_event", GTK_SIGNAL_FUNC(handleKeyPress), this);
	ZLGtkSignalUtil::connectSignal(GTK_OBJECT(myWindow), "key_release_event", GTK_SIGNAL_FUNC(handleKeyRelease), this);*/
}

ZLSymbianApplicationWindow::~ZLSymbianApplicationWindow() {
/*	((ZLGtkDialogManager&)ZLGtkDialogManager::Instance()).setMainWindow(0);
	((ZLMaemoCommunicationManager&)ZLCommunicationManager::Instance()).shutdown();*/
}

void ZLSymbianApplicationWindow::init() {
/*	ZLApplicationWindow::init();
	gtk_main_iteration();
	usleep(5000);
	gtk_main_iteration();
	usleep(5000);
	gtk_main_iteration();*/
}

void ZLSymbianApplicationWindow::initMenu() {
//	MenuBuilder(*this).processMenu(application());
}

ZLSymbianApplicationWindow::MenuBuilder::MenuBuilder(ZLSymbianApplicationWindow &window) : myWindow(window) {
	//myMenuStack.push(myWindow.myMenu);
}

void ZLSymbianApplicationWindow::MenuBuilder::processSubmenuBeforeItems(ZLMenubar::Submenu &submenu) {
  /*
	GtkMenuItem *gtkItem = GTK_MENU_ITEM(gtk_menu_item_new_with_label(submenu.menuName().c_str()));
	GtkMenu *gtkSubmenu = GTK_MENU(gtk_menu_new());
	gtk_menu_item_set_submenu(gtkItem, GTK_WIDGET(gtkSubmenu));
	gtk_menu_shell_append(GTK_MENU_SHELL(myMenuStack.top()), GTK_WIDGET(gtkItem));
	gtk_widget_show_all(GTK_WIDGET(gtkItem));
	myMenuStack.push(gtkSubmenu);
	myWindow.mySubmenuItems.push_back(gtkItem);
  */
}

void ZLSymbianApplicationWindow::MenuBuilder::processSubmenuAfterItems(ZLMenubar::Submenu&) {
	//myMenuStack.pop();
}

void ZLSymbianApplicationWindow::MenuBuilder::processItem(ZLMenubar::PlainItem &item) {
/*	GtkWidget *gtkItem = gtk_button_new_with_label(item.name().c_str());
	const std::string &id = item.actionId();
	shared_ptr<ZLApplication::Action> action = myWindow.application().action(id);
	if (!action.isNull()) {
		ZLGtkSignalUtil::connectSignalAfter(GTK_OBJECT(gtkItem), "clicked", G_CALLBACK(menuActionSlot), &*action);
	}
	myWindow.myMenuItems[id] = gtkItem;
	hildon_app_menu_append(myWindow.myMenu, GTK_BUTTON(gtkItem));
	gtk_widget_show_all(GTK_WIDGET(gtkItem));*/
}

void ZLSymbianApplicationWindow::MenuBuilder::processSepartor(ZLMenubar::Separator&) {
}
/*
bool ZLSymbianApplicationWindow::handleKeyEventSlot(GdkEventKey *event, bool isKeyRelease) {
	const std::string &keyName = ZLGtkKeyUtil::keyName(event);
	if ((myViewWidget != 0) && (KeyActionOnReleaseNotOnPressOption.value() == isKeyRelease)) {
		application().doActionByKey(keyName);
	}
	return keyName == "<Escape>";
}
*/
void ZLSymbianApplicationWindow::setFullscreen(bool fullscreen) {
/*	if (fullscreen == myFullScreen) {
		return;
	}
	myFullScreen = fullscreen;

	if (myFullScreen) {
		gtk_window_fullscreen(GTK_WINDOW(myWindow));
	} else if (!myFullScreen) {
		gtk_window_unfullscreen(GTK_WINDOW(myWindow));
	}*/
}

bool ZLSymbianApplicationWindow::isFullscreen() const {
/*	return myFullScreen;*/return true;
}

void ZLSymbianApplicationWindow::close() {
/*	ZLGtkSignalUtil::removeAllSignals();
	gtk_main_quit();*/
}
/*
static bool itemActivated(GtkWidget *menuItem, gpointer data) {
	GtkMenu *menu = GTK_MENU(gtk_widget_get_parent(menuItem));
	GList *children = gtk_container_get_children(GTK_CONTAINER(menu));
	int index = g_list_index(children, (gconstpointer)menuItem);
	((ZLPopupData*)data)->run(index);
	return true;
}
*//*
void ZLSymbianApplicationWindow::updatePopupData(GtkMenuToolButton*, shared_ptr<ZLPopupData>) {
}
*/
void ZLSymbianApplicationWindow::addToolbarItem(ZLToolbar::ItemPtr) {
}

void ZLSymbianApplicationWindow::setToolbarItemState(ZLToolbar::ItemPtr, bool, bool) {
}

void ZLSymbianApplicationWindow::setToggleButtonState(const ZLToolbar::ToggleButtonItem&) {
}

void ZLSymbianApplicationWindow::present() {
/*	gtk_window_present(GTK_WINDOW(myWindow));*/
}

void ZLSymbianApplicationWindow::refresh() {
/*	ZLApplicationWindow::refresh();

	for (std::map<std::string,GtkMenuItem*>::iterator it = myMenuItems.begin(); it != myMenuItems.end(); it++) {
		const std::string &id = it->first;
		GtkWidget *gtkItem = GTK_WIDGET(it->second);
		if (application().isActionVisible(id)) {
			gtk_widget_show(gtkItem);
		} else {
			gtk_widget_hide(gtkItem);
		}
		bool alreadyEnabled = (GTK_WIDGET_STATE(gtkItem) & GTK_STATE_INSENSITIVE) == 0;
		if (application().isActionEnabled(id) != alreadyEnabled) {
			gtk_widget_set_sensitive(gtkItem, !alreadyEnabled);
		}
	}
	for (std::vector<GtkMenuItem*>::reverse_iterator rit = mySubmenuItems.rbegin(); rit != mySubmenuItems.rend(); ++rit) {
		bool isVisible = false;
		bool isEnabled = false;
		GtkMenu *menu = GTK_MENU(gtk_menu_item_get_submenu(*rit));
		GList *children = gtk_container_get_children(GTK_CONTAINER(menu));
		if (children != 0) {
			for (GList *ptr =  g_list_first(children); ptr != 0; ptr = g_list_next(ptr)) {
				GtkMenuItem *item = GTK_MENU_ITEM(ptr->data);
				if (GTK_WIDGET_VISIBLE(item)) {
					isVisible = true;
					isEnabled = (GTK_WIDGET_STATE(item) & GTK_STATE_INSENSITIVE) == 0;
					if (isEnabled) {
						break;
					}
				}
			}
		}
		if (isEnabled != ((GTK_WIDGET_STATE(*rit) & GTK_STATE_INSENSITIVE) == 0)) {
			gtk_widget_set_sensitive(GTK_WIDGET(*rit), isEnabled);
		}
		if (isVisible) {
			gtk_widget_show(GTK_WIDGET(*rit));
		} else {
			gtk_widget_hide(GTK_WIDGET(*rit));
		}
	}
  */
}

void ZLSymbianApplicationWindow::processAllEvents() {
/*	while (gtk_events_pending()) {
		gtk_main_iteration();
	}*/
}

ZLViewWidget *ZLSymbianApplicationWindow::createViewWidget() {
/*	myViewWidget = new ZLGtkViewWidget(&application(), (ZLView::Angle)application().AngleStateOption.value());
	GtkWidget *area = myViewWidget->area();
	gtk_container_add(GTK_CONTAINER(myWindow), myViewWidget->areaWithScrollbars());
	GtkObject *areaObject = GTK_OBJECT(area);
	ZLGtkSignalUtil::connectSignal(areaObject, "expose_event", GTK_SIGNAL_FUNC(repaint), myViewWidget);
	ZLGtkSignalUtil::connectSignal(areaObject, "button_press_event", GTK_SIGNAL_FUNC(mousePressed), myViewWidget);
	ZLGtkSignalUtil::connectSignal(areaObject, "button_release_event", GTK_SIGNAL_FUNC(mouseReleased), myViewWidget);
	ZLGtkSignalUtil::connectSignal(areaObject, "motion_notify_event", GTK_SIGNAL_FUNC(mouseMoved), myViewWidget);
	gtk_widget_show(GTK_WIDGET(myWindow));

	return myViewWidget;*/
}

void ZLSymbianApplicationWindow::grabAllKeys(bool) {
}

void ZLSymbianApplicationWindow::setFocusToMainWidget() {
/*	gtk_window_set_focus(GTK_WINDOW(myWindow), myViewWidget->area());*/
}
