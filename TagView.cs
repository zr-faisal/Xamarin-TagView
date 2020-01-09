
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V4.Content.Res;
using Android.Util;
using Android.Views;
using Android.Widget;
using static Android.Resource;

namespace HelloXamarin.Droid.View
{
    public class TagView : TextView
    {
        private const string LogTag = "TagView";

        private const int DEFAULT_TEXT_SIZE = 18;
        private const int DEFAULT_VERTICAL_PADDING = 20;
        private const int DEFAULT_HORIZONTAL_PADDING = 40;
        private const int DEFAULT_CORNER_RADIUS = 50;
        private const int DEFAULT_BORDER_WIDTH = 8;

        public bool IsSelected = false;
        public void SetSelected(bool selected)
        {
            this.IsSelected = selected;
            updateState(selected);
            //Invalidate();
        }

        private int CornerRadius = DEFAULT_CORNER_RADIUS;
        public void SetCornerRadius(int cornerRadius)
        {
            this.CornerRadius = cornerRadius;
            Invalidate();
        }

        private int BorderWidth = DEFAULT_BORDER_WIDTH;
        public void SetBorderWidth(int borderWidth)
        {
            this.BorderWidth = borderWidth;
            Invalidate();
        }

        private int DefaultBorderColor = Resource.Color.default_border_color;
        public void SetDefaultBorderColor(int defaultBorderColor)
        {
            this.DefaultBorderColor = defaultBorderColor;
            Invalidate();
        }

        private int DefaultBackgroundColor = Resource.Color.default_background_color;
        public void SetDefaultBackgroundColor(int defaultBackgroundColor)
        {
            this.DefaultBackgroundColor = defaultBackgroundColor;
            Invalidate();
        }

        private int DefaultTextColor = Resource.Color.default_text_color;
        public void SetDefaultTextColor(int defaultTextColor)
        {
            this.DefaultTextColor = defaultTextColor;
            Invalidate();
        }

        private int SelectedBorderColor = Resource.Color.selected_border_color;
        public void SetSelectedBorderColor(int selectedBorderColor)
        {
            this.SelectedBorderColor = selectedBorderColor;
            Invalidate();
        }

        private int SelectedBackgroundColor = Resource.Color.selected_background_color;
        public void SetSelectedBackgroundColor(int selectedBackgroundColor)
        {
            this.SelectedBackgroundColor = selectedBackgroundColor;
            Invalidate();
        }

        private int SelectedTextColor = Resource.Color.selected_text_color;
        public void SetSelectedTextColor(int selectedTextColor)
        {
            this.SelectedTextColor = selectedTextColor;
            Invalidate();
        }

        public TagView(Context context) :
            base(context)
        {
            Initialize(context, null, 0);
        }

        public TagView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize(context, attrs, 0);
        }

        public TagView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize(context, attrs, defStyle);
        }

        void Initialize(Context context, IAttributeSet attributeSet, int defStyle)
        {
            if (context == null)
            {
                return;
            }

            if (attributeSet != null)
            {
                Android.Content.Res.TypedArray typedArray = context
                    .ObtainStyledAttributes(attributeSet, Resource.Styleable.TagView);
                try
                {
                    IsSelected = typedArray.GetBoolean(
                        Resource.Styleable.TagView_selected,
                        false);
                    CornerRadius = typedArray.GetInt(
                        Resource.Styleable.TagView_corner_radius,
                        CornerRadius);
                    BorderWidth = typedArray.GetInt(
                        Resource.Styleable.TagView_border_width,
                        BorderWidth);
                    DefaultBorderColor = typedArray.GetResourceId(
                        Resource.Styleable.TagView_default_border_color,
                        DefaultBorderColor);
                    DefaultBackgroundColor = typedArray.GetResourceId(
                        Resource.Styleable.TagView_default_background_color,
                        DefaultBackgroundColor);
                    DefaultTextColor = typedArray.GetResourceId(
                        Resource.Styleable.TagView_default_text_color,
                        DefaultTextColor);
                    SelectedBorderColor = typedArray.GetResourceId(
                        Resource.Styleable.TagView_selected_border_color,
                        SelectedBorderColor);
                    SelectedBackgroundColor = typedArray.GetResourceId(
                        Resource.Styleable.TagView_selected_background_color,
                        SelectedBackgroundColor);
                    SelectedTextColor = typedArray.GetResourceId(
                        Resource.Styleable.TagView_selected_text_color,
                        SelectedTextColor);
                }
                catch (NotFiniteNumberException e)
                {
                    Log.Error(LogTag, e.Message);
                }
                finally
                {
                    typedArray.Recycle();
                }
            }
            // Set and update view for default state
            SetTextSize(ComplexUnitType.Sp, DEFAULT_TEXT_SIZE);
            SetPadding(
                DEFAULT_HORIZONTAL_PADDING,
                DEFAULT_VERTICAL_PADDING,
                DEFAULT_HORIZONTAL_PADDING,
                DEFAULT_VERTICAL_PADDING);
            updateState(IsSelected);
        }

        private void updateState(bool isSelected)
        {
            SetTextColor(ContextCompat.GetColorStateList(
                Context,
                isSelected ? SelectedTextColor : DefaultTextColor));

            GradientDrawable gradientDrawable = new GradientDrawable();
            gradientDrawable.SetShape(ShapeType.Rectangle);
            gradientDrawable.SetColor(ContextCompat.GetColor(
                Context,
                isSelected ? SelectedBackgroundColor : DefaultBackgroundColor));
            gradientDrawable.SetStroke(
                BorderWidth,
                ContextCompat.GetColorStateList(
                    Context,
                    isSelected ? SelectedBorderColor : DefaultBorderColor));
            gradientDrawable.SetCornerRadius(CornerRadius);
            Background = gradientDrawable;
        }
    }
}
