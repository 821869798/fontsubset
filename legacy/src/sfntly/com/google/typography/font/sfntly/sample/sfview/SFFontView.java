// Copyright 2012 Google Inc. All Rights Reserved.

namespace com.google.typography.font.sfntly.sample.sfview;
















/**
 * @author dougfelt@google.com (Doug Felt)
 */
class SFFontView : JComponent implements Scrollable {
  private static readonly long serialVersionUID = 1L;
  private readonly ViewableTaggedData viewer;

  SFFontView(Font font) {
    setBackground(Color.WHITE);

    PostScriptTable post = font.getTable(Tag.post);
    TaggedDataImpl tdata = new ViewableTaggedData.TaggedDataImpl(post);
    OtTableTagger tagger = new OtTableTagger(tdata);
    GSubTable gsub = font.getTable(Tag.GSUB);
    tagger.tag(gsub);
    viewer = new ViewableTaggedData(tdata.getMarkers());

    Dimension dimensions = viewer.measure(true);

    Dimension minimumSize = new Dimension(400, 400);
    setMinimumSize(minimumSize);
    setPreferredSize(dimensions);
  }

  public override Dimension getPreferredScrollableViewportSize() {
    int width = Math.min(500, viewer.totalWidth());
    int height = 25 * viewer.lineHeight();
    return new Dimension(width, height);
  }

  public override void paintComponent(Graphics g) {
    base.paintComponent(g);
    g.setColor(getBackground());
    g.fillRect(0, 0, getWidth(), getHeight());

    viewer.draw(g, 0, 0);
  }

  public override int getScrollableUnitIncrement(Rectangle visibleRect, int orientation, int direction) {
    if (orientation == SwingConstants.HORIZONTAL) {
      return 50;
    }
    return viewer.lineHeight();
  }

  public override int getScrollableBlockIncrement(Rectangle visibleRect, int orientation, int direction) {
    if (orientation == SwingConstants.HORIZONTAL) {
      return viewer.totalWidth();
    }
    int lines = visibleRect.height / viewer.lineHeight() - 2;
    if (lines < 1) {
      lines = 1;
    }
    return lines * viewer.lineHeight();
  }

  public override boolean getScrollableTracksViewportWidth() {
    return false;
  }

  public override boolean getScrollableTracksViewportHeight() {
    return false;
  }
}
