# Xamarin-TagView
A custom view in Xamarin to generate a grid from list of words.

# Usage
TagView tagView = new TagView(this);
tagView.SetSelected(false);
tagView.SetCornerRadius(8);
tagView.SetBorderWidth(2);
tagView.SetDefaultBorderColor(Color.BLUE);
tagView.SetDefaultBackgroundColor(Color.WHITE);
tagView.SetDefaultTextColor(Color.BLACK);
tagView.SetSelectedBorderColor(Color.RED);
tagView.SetSelectedTextColor(Color.BLUE);

FlowLayout flowLayout = new FlowLayout(this);
flowLayout.addView(tagView);
