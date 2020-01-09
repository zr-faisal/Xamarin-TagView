using System;
namespace HelloXamarin.Droid.View
{
    public interface ITagSelection
    {
        void onTagSelected(int position, TagView view, FlowLayout parent);
    }
}
