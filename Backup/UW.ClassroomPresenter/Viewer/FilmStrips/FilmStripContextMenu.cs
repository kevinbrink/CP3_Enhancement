// $Id: FilmStripContextMenu.cs 1598 2008-04-30 00:49:50Z lining $

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Presentation;
using System.IO;

namespace UW.ClassroomPresenter.Viewer.FilmStrips {
    public class FilmStripContextMenu : ContextMenu {
        private readonly PresenterModel m_Model;
        private readonly DeckTraversalModel m_DeckTraversal;

        public FilmStripContextMenu(DeckTraversalModel traversal, PresenterModel model) {
            this.m_Model = model;
            this.m_DeckTraversal = traversal;
            
            this.MenuItems.Add(new InsertSlideMenuItem(m_DeckTraversal, this.m_Model));
            this.MenuItems.Add(new InsertSlidesFromFileMenuItem(m_DeckTraversal, this.m_Model));
            this.MenuItems.Add(new RemoveSlideMenuItem(m_DeckTraversal, this.m_Model));
        }

        public class InsertSlidesFromFileMenuItem : MenuItem {
            private readonly DeckModel deck_;
            private DeckTraversalModel deck_traversal_;
            private readonly PresenterModel m_Model;

            // Allows the new slide to be added as a child of another TOC entry, or to the main TOC.
            private readonly TableOfContentsModel.EntryCollection m_WhereTheEntriesGo;

            public InsertSlidesFromFileMenuItem(DeckTraversalModel traversal, PresenterModel model)
                : base(Strings.InsertSlidesFromDeck) {
                this.deck_ = traversal.Deck;
                this.deck_traversal_ = traversal;
                this.m_WhereTheEntriesGo = traversal.Deck.TableOfContents.Entries;
                this.m_Model = model;
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);
                InsertDeck();

            }
            private void InsertDeck() {
                OpenFileDialog open_file = new OpenFileDialog();
                open_file.Filter = "CP3, PPT, PPTX files (*.cp3; *.ppt; *.pptx)|*.cp3;*.ppt;*.pptx";

                if (open_file.ShowDialog() == DialogResult.OK) {
                    using (Synchronizer.Lock(this.deck_.SyncRoot)) {
                        this.deck_.Dirty = true;
                    }
                    InsertDeck(new FileInfo(open_file.FileName));
                }
            }
            private void InsertDeck(FileInfo file) {
                DeckModel deck = null;
                if (file.Extension == ".cp3") {
                    deck = Decks.PPTDeckIO.OpenCP3(file);

                } else if (file.Extension == ".ppt") {
                    deck = Decks.PPTDeckIO.OpenPPT(file);
                }
                else if (file.Extension == ".pptx") {
                    deck = Decks.PPTDeckIO.OpenPPT(file);
                }
                if (deck == null) {
                    throw new Exception("Wrong file extension");
                } else {
                    deck_.InsertDeck(deck);
                }

                ///get the index of the current slide that's selected,
                ///so that we can insert our deck there
                int current_slide_index;
                using (Synchronizer.Lock(deck_traversal_.SyncRoot)) {
                    current_slide_index = deck_traversal_.AbsoluteCurrentSlideIndex;
                }

                ///don't do anything if no object is selected
                if (current_slide_index < 0) {
                    return;
                }

                using (Synchronizer.Lock(deck.TableOfContents.SyncRoot)) {
                    using (Synchronizer.Lock(deck_.TableOfContents.SyncRoot)) {
                        ///insert each slide from the deck into our current deck.
                        for (int i = 0; i < deck.TableOfContents.Entries.Count; i++) {
                            ///create our TOC entry
                            TableOfContentsModel.Entry entry = new TableOfContentsModel.Entry(Guid.NewGuid(), deck_.TableOfContents, deck.TableOfContents.Entries[i].Slide);
                            ///insert it into our TOC
                            this.m_WhereTheEntriesGo.Insert(current_slide_index + i, entry);//Natalie - should it be +1?
                        }
                    }
                }

            }
        }
        public class InsertSlideMenuItem : MenuItem {

            private readonly DeckModel m_Deck;

            private DeckTraversalModel traversal_;
            private readonly PresenterModel m_Model;

            // Allows the new slide to be added as a child of another TOC entry, or to the main TOC.
            private readonly TableOfContentsModel.EntryCollection m_WhereTheEntriesGo;

            public InsertSlideMenuItem(DeckTraversalModel traversal, PresenterModel model) : this(traversal.Deck, traversal.Deck.TableOfContents.Entries, traversal, model) {}

            public InsertSlideMenuItem(DeckModel deck, TableOfContentsModel.EntryCollection bucket, DeckTraversalModel traversal, PresenterModel model) : base(Strings.NewSlide) {
                this.m_Deck = deck;
                this.m_WhereTheEntriesGo = bucket;
                this.traversal_ = traversal;
                this.m_Model = model;
                // TODO: Disable this menu item if the deck is immutable (requires defining what sorts of decks are mutable or not).
            }

            protected override void OnClick(EventArgs e) {
                base.OnClick(e);
                InsertSlide();
            }
            /// <summary>
            /// inserts a blank slide in the current deck at the selected index. If index is
            /// not selected, does nothing. THis is a seperate method because it will be called
            /// from outside the contextmenu
            /// </summary>
            private void InsertSlide() {
                ///insert the new slide into our deck
                SlideModel slide;
                using (Synchronizer.Lock(this.m_Deck.SyncRoot)) {
                    slide = new SlideModel(Guid.NewGuid(), new LocalId(), SlideDisposition.Empty, UW.ClassroomPresenter.Viewer.ViewerForm.DEFAULT_SLIDE_BOUNDS);
                    this.m_Deck.Dirty = true;
                    this.m_Deck.InsertSlide(slide);
                }

                ///get the index of the current slide that's selected,
                ///so that we can insert our blank slide there
                int current_slide_index;
                using (Synchronizer.Lock(traversal_.SyncRoot)) {
                    current_slide_index = traversal_.AbsoluteCurrentSlideIndex;
                }

                ///don't do anything if no object is selected
                if (current_slide_index < 0) {
                    return;
                }

                ///Insert our blank slide after the current index. This is modeled after the powerpoint
                ///UI
                using (Synchronizer.Lock(this.m_Deck.TableOfContents.SyncRoot)) {
                    TableOfContentsModel.Entry entry = new TableOfContentsModel.Entry(Guid.NewGuid(), this.m_Deck.TableOfContents, slide);
                    this.m_WhereTheEntriesGo.Insert(current_slide_index, entry);
                }
            }
        }
        public class RemoveSlideMenuItem : MenuItem {

            private readonly DeckModel m_Deck;

            private DeckTraversalModel traversal_;
            private readonly PresenterModel m_Model;

            // Allows the new slide to be added as a child of another TOC entry, or to the main TOC.
            private readonly TableOfContentsModel.EntryCollection m_WhereTheEntriesGo;

            /// <summary>
            /// Creates ne option to delete the current slide.
            /// </summary>
            /// <param name="traversal"></param>
            public RemoveSlideMenuItem(DeckTraversalModel traversal, PresenterModel model) : base(Strings.DeleteSlide){
                this.m_Deck = traversal.Deck;
                this.m_WhereTheEntriesGo = traversal.Deck.TableOfContents.Entries;
                traversal_ = traversal;
                this.m_Model = model;
            }

            /// <summary>
            /// Remove the currently selected slide
            /// </summary>
            /// <param name="e"></param>
            protected override void OnClick(EventArgs e) {
                base.OnClick(e);
                RemoveSlide();
            }

            /// <summary>
            /// Removes a slide from the current deck at the current index.
            /// If no index is selected, does nothing. This is public because
            /// it will be accessed outside the context menu
            /// </summary>
            private void RemoveSlide() {
                ///get the index of the current slide that's selected,
                ///so that we can remove our blank slide
                int current_slide_index;
                using (Synchronizer.Lock(traversal_.SyncRoot)) {
                    ///-1 because of zero based indexing?
                    current_slide_index = traversal_.AbsoluteCurrentSlideIndex - 1;
                }

                ///don't do anything if no object is selected
                if (current_slide_index < 0) {
                    return;
                }
                //Update the current slide
                using (Synchronizer.Lock(this.traversal_.SyncRoot)) {
                    if (current_slide_index + 1 == this.m_WhereTheEntriesGo.Count) {
                        this.traversal_.Current = this.traversal_.Previous;
                    }
                    else {
                        this.traversal_.Current = this.traversal_.Next;
                    }
                }
                ///Remove slide from the Deck
                using (Synchronizer.Lock(m_Deck.SyncRoot)) {
                    this.m_Deck.Dirty = true;
                    m_Deck.DeleteSlide(m_WhereTheEntriesGo[current_slide_index].Slide);
                }

                ///Remove slide at current index from the TOC
                using (Synchronizer.Lock(this.m_Deck.TableOfContents.SyncRoot)) {
                    this.m_WhereTheEntriesGo.RemoveAt(current_slide_index);
                }  
                
            }
        }
    }
}
