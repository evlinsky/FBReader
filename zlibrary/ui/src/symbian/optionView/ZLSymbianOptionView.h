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

#ifndef __ZLSYMBIANOPTIONVIEW_H__
#define __ZLSYMBIANOPTIONVIEW_H__

/*#include <gtk/gtk.h>

#if MAEMO_VERSION <= 3
	#include <hildon-controlbar.h>
	#include <hildon-number-editor.h>
#elif MAEMO_VERSION == 4
	#include <hildon/hildon-controlbar.h>
	#include <hildon/hildon-number-editor.h>
#elif MAEMO_VERSION == 5
  #include <hildon/hildon.h>
#else
	#error Unknown MAEMO_VERSION
#endif
*/
#include <ZLOptionsDialog.h>
#include <ZLOptionEntry.h>
#include "../../../../core/src/dialogs/ZLOptionView.h"

class ZLSymbianOptionViewHolder;

class ZLSymbianOptionView : public ZLOptionView {

protected:
	ZLSymbianOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLOptionView(name, tooltip, option), myHolder(holder) {}

protected:
//	static void _onValueChanged(GtkWidget*, gpointer self);
	// TODO: replace by pure virtual method (?)
	virtual void onValueChanged() {}

protected:
	ZLSymbianOptionViewHolder &myHolder;
};

class ChoiceOptionView : public ZLSymbianOptionView {

public:
	ChoiceOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {
/*		myFrame = 0;*/
	}
	~ChoiceOptionView() { /*if (myFrame != 0) delete[] myButtons;*/ }

protected:
	void _createItem();
	void _show();
	void _hide();
	void _setActive(bool active);
	void _onAccept() const;

private:
//	GtkFrame *myFrame;
//	GtkBox *myVBox;
//	GtkRadioButton **myButtons;
};

class BooleanOptionView : public ZLSymbianOptionView {

public:
	BooleanOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {}

protected:
	void _createItem();
	void _show();
	void _hide();
	void _setActive(bool active);
	void _onAccept() const;

private:
	void onValueChanged();

private:
//	GtkWidget *myCheckBox;
};

class Boolean3OptionView : public ZLSymbianOptionView {

private:
	static ZLBoolean3 stateByIndex(int index);

public:
	Boolean3OptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {}

protected:
	void _createItem();
	void _show();
	void _hide();
	void _setActive(bool active);
	void _onAccept() const;
	void reset();

private:
	void onValueChanged();

private:
//	GtkLabel *myLabel;
//	GtkComboBox *myComboBox;
};

class StringOptionView : public ZLSymbianOptionView {

public:
	StringOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {}

private:
	void _createItem();
	void _show();
	void _hide();
	void _setActive(bool active);
	void _onAccept() const;
	void reset();
	void onValueChanged();

private:
//	GtkLabel *myLabel;
//	HildonEntry *myLineEdit;
};

class MultilineOptionView : public ZLSymbianOptionView {

public:
	MultilineOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {}

private:
	void _createItem();
	void _show();
	void _hide();
	void _setActive(bool active);
	void _onAccept() const;
	void reset();
	void onValueChanged();

private:
//	GtkTextBuffer *myTextBuffer;
//	GtkTextView *myTextView;
};

class SpinOptionView : public ZLSymbianOptionView {

public:
	SpinOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {}

protected:
	void _createItem();
	void _show();
	void _hide();
	void _onAccept() const;

private:
//	GtkLabel *myLabel;
//	HildonNumberEditor *mySpinBox;
};

class ComboOptionView : public ZLSymbianOptionView {

public:
	ComboOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder), /*myPickerButton(0),*/ myListSize(0) {}

private:
	void _createItem();
	void _show();
	void _hide();
	void _setActive(bool active);
	void _onAccept() const;
	void reset();
	void onValueChanged();
	
private:
//	GtkWidget *myPickerButton;
	int mySelectedIndex;
	int myListSize;
};

class ColorOptionView : public ZLSymbianOptionView {

public:
	ColorOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {}

private:
	void _createItem();
	void _show();
	void _hide();
	void _onAccept() const;
	void reset();

	void onSliderMove();
//	static void _onSliderMove(GtkRange *, gpointer);

private:
//	GtkTable *myTable;
//	GtkWidget *myDrawingArea;
//	HildonControlbar *myRSlider, *myGSlider, *myBSlider;
//	GdkColor myColor;
};

class KeyOptionView : public ZLSymbianOptionView {

public:
	KeyOptionView(const std::string &name, const std::string &tooltip, shared_ptr<ZLOptionEntry> option, ZLSymbianOptionViewHolder &holder) : ZLSymbianOptionView(name, tooltip, option, holder) {}

	void setKey(const std::string &key);

private:
	void _createItem();
	void _show();
	void _hide();
	void _onAccept() const;
	void onValueChanged();
	void reset();

private:
//	GtkTable *myTable;
//	GtkEntry *myKeyEntry;
//	GtkLabel *myLabel;
//	GtkComboBox *myComboBox;
	std::string myCurrentKey;
};


#endif /* __ZLSYMBIANOPTIONVIEW_H__ */
