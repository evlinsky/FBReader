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

#include <algorithm>

// el #include <linebreak.h>

#include <ZLImage.h>

#include <ZLTextParagraph.h>

#include "ZLTextParagraphCursor.h"
#include "ZLTextWord.h"
#include "ZLTextParagraphBuilder.h"

ZLTextParagraphCursor::Builder::Builder(ZLTextParagraphCursor &cursor) :
	myParagraph(*cursor.myModel[cursor.myIndex]),
	myElements(cursor.myElements),
	myLanguage(cursor.myModel.language())/* el,
	myBaseBidiLevel(cursor.myModel.isRtl() ? 1 : 0)  */ {
	const int paragraphIndex = cursor.myIndex;
	const std::vector<ZLTextMark> &marks = cursor.myModel.marks();
	myFirstMark = std::lower_bound(marks.begin(), marks.end(), ZLTextMark(paragraphIndex, 0, 0));
	myLastMark = myFirstMark;
	for (; (myLastMark != marks.end()) && (myLastMark->ParagraphIndex == paragraphIndex); ++myLastMark) {
	}
	myOffset = 0;

	static bool lineBreakInitialized = false;
	if (!lineBreakInitialized) {
// el		init_linebreak();
		lineBreakInitialized = true;
	}
}


/* el
void ZLTextParagraphCursor::Builder::updateBidiLevel(FriBidiLevel bidiLevel) {
	while (myCurrentBidiLevel > bidiLevel) {
		--myCurrentBidiLevel;
		myElements.push_back(ZLTextElementPool::Pool.EndReversedSequenceElement);
	}
	while (myCurrentBidiLevel < bidiLevel) {
		++myCurrentBidiLevel;
		myElements.push_back(ZLTextElementPool::Pool.StartReversedSequenceElement);
	}
}
*/

void ZLTextParagraphCursor::Builder::addWord(const char *ptr, int offset, int len) {
// el	ZLTextWord *word = ZLTextElementPool::Pool.getWord(ptr, len, offset, myCurrentBidiLevel);
// el	for (std::vector<ZLTextMark>::const_iterator mit = myFirstMark; mit != myLastMark; ++mit) {
// el		ZLTextMark mark = *mit;
// el		if ((mark.Offset < offset + len) && (mark.Offset + mark.Length > offset)) {
// el			word->addMark(mark.Offset - offset, mark.Length);
// el		}
// el	}
// el	myElements.push_back(word);
}

void ZLTextParagraphCursor::Builder::fill() {
// el	myBidiCharType = (myBaseBidiLevel == 1) ? FRIBIDI_TYPE_RTL : FRIBIDI_TYPE_LTR;
// el	myCurrentBidiLevel = myBaseBidiLevel;
// el	myLatestBidiLevel = myBaseBidiLevel;

	for (ZLTextParagraph::Iterator it = myParagraph; !it.isEnd(); it.next()) {
		switch (it.entryKind()) {
			case ZLTextParagraphEntry::STYLE_ENTRY:
				myElements.push_back(new ZLTextStyleElement(it.entry()));
				break;
			case ZLTextParagraphEntry::FIXED_HSPACE_ENTRY:
				myElements.push_back(new ZLTextFixedHSpaceElement(((ZLTextFixedHSpaceEntry&)*it.entry()).length()));
				break;
			case ZLTextParagraphEntry::CONTROL_ENTRY:
			case ZLTextParagraphEntry::HYPERLINK_CONTROL_ENTRY:
				myElements.push_back(ZLTextElementPool::Pool.getControlElement(it.entry()));
				break;
			case ZLTextParagraphEntry::IMAGE_ENTRY:
			{
				ImageEntry &imageEntry = (ImageEntry&)*it.entry();
				shared_ptr<const ZLImage> image = imageEntry.image();
				if (!image.isNull()) {
					shared_ptr<ZLImageData> data = ZLImageManager::Instance().imageData(*image);
					if (!data.isNull()) {
						myElements.push_back(new ZLTextImageElement(imageEntry.id(), data));
					}
				}
				break;
			}
			case ZLTextParagraphEntry::TEXT_ENTRY:
				processTextEntry((const ZLTextEntry&)*it.entry());
				break;
			case ZLTextParagraphEntry::RESET_BIDI_ENTRY:
// el				updateBidiLevel(myBaseBidiLevel);
// el				myLatestBidiLevel = myBaseBidiLevel;
				break;
		}
	}

// el	updateBidiLevel(myBaseBidiLevel);
}

void ZLTextParagraphCursor::Builder::processTextEntry(const ZLTextEntry &textEntry) {
	const size_t dataLength = textEntry.dataLength();
	if (dataLength == 0) {
		return;
	}

	myUcs4String.clear();
	ZLUnicodeUtil::utf8ToUcs4(myUcs4String, textEntry.data(), dataLength);
	int len = myUcs4String.size();
	myUcs4String.push_back(0);
// el	myBidiLevels.clear();
// el	myBidiLevels.assign(len + 1, 0);
	int firstNonSpace = 0;
	while ((firstNonSpace < len) &&
				 ZLUnicodeUtil::isSpace(myUcs4String[firstNonSpace])) {
// el		myBidiLevels[firstNonSpace++] = myLatestBidiLevel;
	}
	int lastNonSpace = len - 1;
	if (lastNonSpace > firstNonSpace - 1) {
		while (ZLUnicodeUtil::isSpace(myUcs4String[lastNonSpace])) {
			--lastNonSpace;
		}
// el		fribidi_log2vis((FriBidiChar*)&myUcs4String[firstNonSpace], lastNonSpace - firstNonSpace + 1, &myBidiCharType, 0, 0, 0, &myBidiLevels[firstNonSpace]);
	}
// el	myLatestBidiLevel = myBidiLevels[lastNonSpace];
	for (int i = lastNonSpace; i < len; ++i) {
// el		myBidiLevels[i] = myLatestBidiLevel;
	}

	myBreaksTable.clear();
	myBreaksTable.assign(dataLength, 0);
	const char *start = textEntry.data();
	const char *end = start + dataLength;
// el	set_linebreaks_utf8((const utf8_t*)start, dataLength, myLanguage.c_str(), &myBreaksTable[0]);

	ZLUnicodeUtil::Ucs4Char ch = 0, previousCh;
	enum { NO_SPACE, SPACE, NON_BREAKABLE_SPACE } spaceState = NO_SPACE;
	int charLength = 0;
	int index = 0;
	const char *wordStart = start;
// el	updateBidiLevel(myBidiLevels[0]);
	for (const char *ptr = start; ptr < end; ptr += charLength, ++index) {
		previousCh = ch;
		charLength = ZLUnicodeUtil::firstChar(ch, ptr);
		if (ZLUnicodeUtil::isSpace(ch)) {
			if ((spaceState == NO_SPACE) && (ptr != wordStart)) {
				addWord(wordStart, myOffset + (wordStart - start), ptr - wordStart);
			}
			spaceState = SPACE;
		} else if (ZLUnicodeUtil::isNBSpace(ch)) {
			if (spaceState == NO_SPACE) {
				if (ptr != wordStart) {
					addWord(wordStart, myOffset + (wordStart - start), ptr - wordStart);
				}
				spaceState = NON_BREAKABLE_SPACE;
			}
		} else {
			switch (spaceState) {
				case SPACE:
// el					if ((myBreaksTable[ptr - start - 1] == LINEBREAK_NOBREAK) || (previousCh == '-')) {
// el						myElements.push_back(ZLTextElementPool::Pool.NBHSpaceElement);
// el					} else {
// el						myElements.push_back(ZLTextElementPool::Pool.HSpaceElement);
// el					}
					wordStart = ptr;
					break;
				case NON_BREAKABLE_SPACE:
					myElements.push_back(ZLTextElementPool::Pool.NBHSpaceElement);
					wordStart = ptr;
					break;
				case NO_SPACE:
// el					if ((ptr > start) &&
// el							((((myBreaksTable[ptr - start - 1] != LINEBREAK_NOBREAK) && (previousCh != '-')) && (ptr != wordStart)) ||
// el							 (myBidiLevels[index - 1] != myBidiLevels[index]))) {
// el						addWord(wordStart, myOffset + (wordStart - start), ptr - wordStart);
// el						wordStart = ptr;
// el					}
					break;
			}
			spaceState = NO_SPACE;
		}
// el		updateBidiLevel(myBidiLevels[index]);
	}
	switch (spaceState) {
		case SPACE:
			myElements.push_back(ZLTextElementPool::Pool.HSpaceElement);
			break;
		case NON_BREAKABLE_SPACE:
			myElements.push_back(ZLTextElementPool::Pool.NBHSpaceElement);
			break;
		case NO_SPACE:
			addWord(wordStart, myOffset + (wordStart - start), end - wordStart);
			break;
	}
	myOffset += dataLength;
}
