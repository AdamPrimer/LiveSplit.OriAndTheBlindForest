# Ori and the Blind Forest Autosplitter #

## Downloading ##

1. Go to the releases page:

    https://github.com/AdamPrimer/LiveSplit.OriAndTheBlindForest/releases

2. Download `LiveSplit.OriAndTheBlindForest.dll`

## Installing ##

1. Close LiveSplit completely
2. Place `LiveSplit.OriAndTheBlindForest.dll` in the `Components` directory which is
inside your `LiveSplit` directory.
3. Open LiveSplit
4. Open your splits
5. Right click -> Edit Layout
6. Click the + symbol -> Control -> Ori and the Blind Forest Autosplitter

## About the Autosplitter ##

1. Autosplits occur in a linear order.
2. The splits in the autosplitter are NOT YOUR SPLITS
3. The autosplits occur are customisable and even new autosplits can be created.
4. This document is long, but will let you get the most out of the
   autosplitter, so please read it! We added a bunch of powerful features we
   think you'll really appreciate to get the most out!

## Configuring ##

__For each of the autosplits added to this config page, you must have an actual
split in your splits to match it__

1. Double Click `Ori and the Blind Forest Autosplitter` in the `Layout
   Editor`
2. You can add a bunch of default split conditions for your route by clicking
   the `Any %`, `100%` (and the like) buttons.
3. Otherwise you can add a new split by clicking `Add Split`.
4. You can move splits up and down by using the arrows. You can delete a split
   by clicking the X.

Firstly, be aware there are two different sets of splits. There are your
splits, and the autosplitter autosplits. It is up to you to make sure they are
synchronized. This gives you maximal control over when autosplits or manual
splits are used. 

An event triggered added here will cause LiveSplit to trigger "Split()" when
true, it doesn't care what splits you have configured or what they are named,
it just tells LiveSplit to split like you would by hitting a Split key.

Some autosplit events are special.

The numbers next to some splits such as `Iceless` are coordinates for a hitbox.
When Ori intersects this hitbox, the split will occur. See more about
`Hitboxes` below.

Some splits such as `Keystones` take a value as well, see `Values` below.

## Hitboxes ##

The `Hitbox` split type is very powerful, and slightly complicated.

Hitboxes can be used to create your own custom splits based on Ori entering a
certain location on the screen. This can be used to create splits for lava
skips, or Gumo's trap, or all manner of custom events.

### Visualising Hitboxes ###

- Pick the `Iceless` split, then click in the box with the numbers next to it.
- Alt-tab to Ori, make sure the game is windowed not fullscreen.
- You should see numbers in the bottom right corner of the screen if it worked.
- If you go to the location of Iceless, you will also see a Red box rendered on
  the screen.
- Congratulations, you just visualized a hitbox! 

### Creating a hitbox ###

- Pick the `Hitbox` split, click in the blank box next to it.
- Alt-tab to the game as done above.
- Hold down middle click at a point on the screen
- Drag the mouse while holding middle-mouse to draw a hitbox.
- Congratulations, you just made a hitbox!

## Values ##

Some split types allow you to provide a value, such as `Keystones` and `Health
Cells`. A split will trigger when the value of that is equal to what you
provide.

### Creating a value split ###

- Pick a `Keystones` split, click the box with a `1` in it next to it.
- Type the number `2`.
- Congratulations, this split will trigger when you have two keystones!

__NOTE:__ Due to the strict linearity of the splits, which two keystones it
triggers on will depend on where the split is placed. If it is placed after a
`Spirit Flame` split, it will trigger on the first two keystones you ever get. If
it is placed after a `Glide` split, it will trigger on the first two keystones
you pick up after that. 
