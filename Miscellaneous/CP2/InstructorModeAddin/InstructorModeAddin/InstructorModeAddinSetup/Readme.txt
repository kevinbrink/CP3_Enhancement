README for Instructor Mode Addin

*******************************************
DESCRIPTION
*******************************************

This is a beta of a PowerPoint add-in which manages content visible
only to the instructor to facilitate creating "instructor notes" in
Presenter presentations. The addin gives you two toolbars, the 
Presenter View Mode toolbar and the Presenter Object toolbar, which 
control what content is visible and to whom.

For now, let's consider only the Presenter View Mode toolbar. You 
can think of "Projected View" mode as regular old PowerPoint. All 
your normal content is visible in base mode. By default, your 
presentation will start in this mode. 

Each of the other modes, then, is like a glass pane that you can 
place over the entire presentation. For example, when the instructor
mode pane is down, you are in instructor mode, and the Instructor
button is highlighted. To switch between modes, just click the 
button of the mode you wish to use. (Note: most people will 
probably only need to use the Instructor and Base modes.)

That glass pane can have all the normal pieces of a PowerPoint
presentation on it. So, when it's down, you can see those pieces, and
when it's up, you can't. Furthermore, while it's down, anything new
you place in your presentation (including default elements of new
slides!) falls on top of the glass pane, and will be lifted up next
time you raise the pane.

This means that you can create content visible only to the instructor
(or, with the Student and Shared modes, visible only to the students 
or on the projected display). Note that a good way to keep track of
what content is in what mode is to flip between Base mode and the 
mode you're interested in. You can watch the thumbnail slides and see
what content changes.

Now, back to the restriction toolbar. An object is restricted if it 
is on one of the glass panes. When you select an object, buttons
on the restriction toolbar highlight to reflect the object's 
restriction. If the object is unrestricted, only the unrestricted
button will light up. If the object is restricted to one or more
modes, the buttons for those modes will light up. (Note: if an 
object is restricted to more than one mode, it just means it will
be visible in any of those modes.)

If you select an object in your presentation and then click a button 
on the restriction toolbar, you will change the restriction of those
objects. Selecting the "Unrestricted" button will always make 
selected shapes unrestricted. Otherwise, clicking a highlighted 
button will turn off restriction to that mode while clicking an 
unhighlighted button will turn on restriction to that mode. Selecting
multiple objects works the same way except that if some of the objects
are restricted to a mode and others aren't, the mode's button will
be unhighlighted.

Finally, you can export your presentation to be used in Presenter by
clicking the "Export to CSD" button under the File menu. In Presenter,
the instructor pane will automatically be shown in instructor mode,
the student pane will be shown in viewer mode, and the shared pane
in projector mode. (Apologies for the terminology drift!)

One more note: if you save and reload your PowerPoint file, you will
find that all your restricted mode objects are still there and their
restriction status has been saved. All the information related to
restriction is stored in the PowerPoint file; so, as long as you
save your file, you will not lose your restricted objects.

*******************************************
SUMMARY
*******************************************

To summarize:

- base mode: no restricted objects are visible, and new objects 
  are unrestricted, and

- instructor (plus student and shared) mode: all unrestricted 
  objects are visible as are objects restricted to instructor (or,
  respectively, student or shared mode)

- mode toolbar: indicates the current mode and switches it on clicks

- restriction toolbar: indicates the restriction of the current
  selection and switches it on clicks

*******************************************
KNOWN ISSUES
*******************************************

Problems with the current version: It will probably only work
w/PowerPoint XP. Also, the toolbars will always reposition 
thesmelves into the same, stubbornly inappropriate spot each time
you reopen PowerPoint. We have had reports that other customizations
of toolbars may be lost when using the add-in as well. Also, some
oddness seems to occur with print setup on some machines: rather
than asking for printing preferences, PowerPoint will just print
immediately with the defaults. You can work around this if it 
happens to you by using the Print Preview view to adjust printing
preferences. Finally, selecting text inside an object and restricting
the object's visibility will have slightly odd visual effects. 
(Parts of the object will disappear but the text will remain visible
and editable.) These effects should disappear either after the
mode is changed to the new mode the object is restricted to or after
changing slides.

Also, there is a known PowerPoint bugs exposed by the add-in which 
deals with grouping objects. In the first, grouping two objects and
then grouping a third with the result may crash PPT although PPT's
autorecovery feature reliably saves your presentation.  

There are probably many UNKNOWN bugs! 

(If by some chance the add-in fails catastrophically and you're 
desperate to retrieve your now hidden (and inaccessible) restricted
objects, you can use some simple VB code to reveal them. You will
need to iterate over all objects in all slides, setting the Visible
property of each object to true. You can also find such "lost" 
objects by tabbing to them in the UI.)

Performancewise, the add-in chugs for a while when loading a large
presentation. (Chugging time is approx. proportional to the number of
shapes in the presentation and ran at 10-15 seconds on my old
PII-500MHz machine with 256MB of RAM.) Exporting large files can take
even longer but should take no more time (and probably actually less
time) than DeckBuilder. The PowerPoint executable may also continue 
running invisibly even after you close it.

Steve Wolfman
July 25, 2003
