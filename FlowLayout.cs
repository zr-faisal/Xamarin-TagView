
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace HelloXamarin.Droid.View
{
    public class FlowLayout : ViewGroup
    {
        private const string LogTag = "FlowLayout";

        private const int SPACING_AUTO = -99999;
        private const int SPACING_ALIGN = -99998;
        private const int SPACING_UNDEFINED = -99997;
        private const int UNSPECIFIED_GRAVITY = -1;

        private const bool DEFAULT_FLOW = true;
        private const int DEFAULT_CHILDSPACING = 10;
        private const int DEFAULT_CHILDSPACING_FOR_LASTROW = SPACING_UNDEFINED;
        private const int DEFAULT_ROWSPACING = 10;
        private const int DEFAULT_MAXROWS = int.MaxValue;

        private int exactMeasuredHeight = 0;
        private int adjustedRowSpacing = DEFAULT_ROWSPACING;

        private List<int> horizontalSpacingForRow = new List<int>();
        private List<int> heightForRow = new List<int>();
        private List<int> widthForRow = new List<int>();
        private List<int> childNumberForRow = new List<int>();

        private bool Flow = DEFAULT_FLOW;
        public void SetFlow(bool flow)
        {
            this.Flow = flow;
            RequestLayout();
        }

        private int ChildSpacing = DEFAULT_CHILDSPACING;
        public void SetChildSpacing(int childSpacing)
        {
            this.ChildSpacing = childSpacing;
            RequestLayout();
        }

        private int MinChildSpacing = DEFAULT_CHILDSPACING;
        public void SetMinChildSpacing(int minChildSpacing)
        {
            this.MinChildSpacing = minChildSpacing;
            RequestLayout();
        }

        private int ChildSpacingForLastRow = DEFAULT_CHILDSPACING_FOR_LASTROW;
        public void SetChildSpacingForLastRow(int childSpacingForLastRow)
        {
            this.ChildSpacingForLastRow = childSpacingForLastRow;
            RequestLayout();
        }

        private int RowSpacing = DEFAULT_ROWSPACING;
        public void SetRowSpacing(int rowSpacing)
        {
            this.RowSpacing = rowSpacing;
            RequestLayout();
        }

        private int MaxRows = DEFAULT_MAXROWS;
        public void SetMaxRows(int maxRows)
        {
            this.MaxRows = maxRows;
            RequestLayout();
        }

        private int Gravity = UNSPECIFIED_GRAVITY;
        public void SetGravity(int gravity)
        {
            this.Gravity = gravity;
            RequestLayout();
        }

        public bool RemoveSelection { get; set; } = false;

        public ITagSelection TagSelectionHandler { get; set; } = null;

        public FlowLayout(Context context) : base(context)
        {
            Initialize(context, null, 0);
        }

        public FlowLayout(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(context, attrs, 0);
        }

        public FlowLayout(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
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
                Android.Content.Res.TypedArray typedArray =
                    context.ObtainStyledAttributes(
                               attributeSet,
                               Resource.Styleable.FlowLayout);
                try
                {
                    Flow = typedArray.GetBoolean(
                        Resource.Styleable.FlowLayout_delfault_flow, DEFAULT_FLOW);
                    ChildSpacing = typedArray.GetInt(
                        Resource.Styleable.FlowLayout_child_spacing,
                        (int)DpToPx(DEFAULT_CHILDSPACING));
                    MinChildSpacing = typedArray.GetInt(
                        Resource.Styleable.FlowLayout_min_child_spacing,
                        (int)DpToPx(DEFAULT_CHILDSPACING));
                    ChildSpacingForLastRow = typedArray.GetInt(
                        Resource.Styleable.FlowLayout_child_spacing_last_row,
                        (int)DpToPx(DEFAULT_CHILDSPACING));
                    RowSpacing = typedArray.GetInt(
                        Resource.Styleable.FlowLayout_row_spacing,
                        (int)DpToPx(DEFAULT_ROWSPACING));
                    MaxRows = typedArray.GetInt(
                        Resource.Styleable.FlowLayout_max_rows,
                        DEFAULT_MAXROWS);
                    Gravity = typedArray.GetInt(
                        Resource.Styleable.FlowLayout_android_gravity,
                        UNSPECIFIED_GRAVITY);
                    RemoveSelection = typedArray.GetBoolean(
                        Resource.Styleable.FlowLayout_remove_selection,
                        RemoveSelection);
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
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            int heightSize = MeasureSpec.GetSize(heightMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            horizontalSpacingForRow.Clear();
            heightForRow.Clear();
            widthForRow.Clear();
            childNumberForRow.Clear();

            int measuredWidth = 0;
            int measuredHeight = 0;
            int childCount = ChildCount;

            int rowWidth = 0;
            int maxChildHeightInRow = 0;
            int childNumberInRow = 0;

            int rowSize = widthSize - PaddingLeft - PaddingRight;
            int rowTotalChildWidth = 0;

            bool allowFlow = widthMode != MeasureSpecMode.Unspecified && Flow;
            int childSpacing = ChildSpacing == SPACING_AUTO && widthMode
                == MeasureSpecMode.Unspecified ? 0 : ChildSpacing;
            int tmpSpacing = childSpacing
                == SPACING_AUTO ? MinChildSpacing : childSpacing;

            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChildAt(i);
                if (child.Visibility == ViewStates.Gone)
                {
                    continue;
                }

                LayoutParams childLayoutParams = child.LayoutParameters;
                int horizontalMargin = 0;
                int verticalMargin = 0;

                if (childLayoutParams is MarginLayoutParams)
                {
                    MeasureChildWithMargins(child, widthMeasureSpec, 0, heightMeasureSpec, measuredHeight);

                    MarginLayoutParams marginLayoutParams = (MarginLayoutParams)childLayoutParams;
                    horizontalMargin = marginLayoutParams.LeftMargin + marginLayoutParams.RightMargin;
                    verticalMargin = marginLayoutParams.TopMargin + marginLayoutParams.BottomMargin;
                }
                else
                {
                    MeasureChild(child, widthMeasureSpec, heightMeasureSpec);
                }

                int childWidth = child.MeasuredWidth + horizontalMargin;
                int childHeight = child.MeasuredHeight + verticalMargin;

                // Check if need to flow to the next row
                if (allowFlow && rowWidth + childWidth > rowSize)
                {
                    // Save parameters for current row
                    horizontalSpacingForRow.Add(
                        SpacingForRow(
                            childSpacing,
                            rowSize,
                            rowTotalChildWidth,
                            childNumberInRow));
                    childNumberForRow.Add(childNumberInRow);
                    heightForRow.Add(maxChildHeightInRow);
                    widthForRow.Add(rowWidth - tmpSpacing);

                    measuredWidth = Math.Max(measuredWidth, rowWidth);
                    if (horizontalSpacingForRow.Count <= MaxRows)
                    {
                        measuredHeight += maxChildHeightInRow;
                    }

                    // Pleace the child view to next row
                    childNumberInRow = 1;
                    rowWidth = childWidth + tmpSpacing;
                    rowTotalChildWidth = childWidth;
                    maxChildHeightInRow = childHeight;
                }
                else
                {
                    childNumberInRow++;
                    rowWidth += childWidth + tmpSpacing;
                    rowTotalChildWidth += childWidth;
                    maxChildHeightInRow = Math.Max(maxChildHeightInRow, childHeight);
                }
            }

            // Measure remaining child views in the last row
            if (ChildSpacingForLastRow == SPACING_ALIGN)
            {
                // For SpacingAlign, use the same spacing from the row above if there is more than 1 row.
                if (horizontalSpacingForRow.Count >= 1)
                {
                    horizontalSpacingForRow.Add(
                        horizontalSpacingForRow
                        .ElementAt(horizontalSpacingForRow.Count - 1));
                }
                else
                {
                    horizontalSpacingForRow.Add(
                        SpacingForRow(
                            childSpacing,
                            rowSize,
                            rowTotalChildWidth,
                            childNumberInRow));
                }
            }
            else if (ChildSpacingForLastRow != SPACING_UNDEFINED)
            {
                // For SpacingAuto and specific DP values, apply them to the spacing strategy.
                horizontalSpacingForRow.Add(
                    SpacingForRow(
                        ChildSpacingForLastRow,
                        rowSize,
                        rowTotalChildWidth,
                        childNumberInRow));
            }
            else
            {
                // For SpacingUndefined, apply childSpacing to the spacing strategy for the last row.
                horizontalSpacingForRow.Add(
                    SpacingForRow(
                        childSpacing,
                        rowSize,
                        rowTotalChildWidth,
                        childNumberInRow));
            }

            childNumberForRow.Add(childNumberInRow);
            widthForRow.Add(rowWidth - tmpSpacing);
            heightForRow.Add(maxChildHeightInRow);

            measuredWidth = Math.Max(measuredWidth, rowWidth);
            if (horizontalSpacingForRow.Count <= MaxRows)
            {
                measuredHeight += maxChildHeightInRow;
            }

            if (childSpacing == SPACING_AUTO)
            {
                measuredWidth = widthSize;
            }
            else if (widthMode == MeasureSpecMode.Unspecified)
            {
                measuredWidth = measuredWidth + PaddingLeft + PaddingRight;
            }
            else
            {
                measuredWidth = Math.Min(
                    measuredWidth + PaddingLeft + PaddingRight,
                    widthSize);
            }
            measuredHeight += PaddingTop + PaddingBottom;

            int rowNumber = Math.Min(horizontalSpacingForRow.Count, MaxRows);
            int rowSpacing = RowSpacing == SPACING_AUTO && heightMode
                == MeasureSpecMode.Unspecified ? 0 : RowSpacing;

            if (rowSpacing == SPACING_AUTO)
            {
                if (rowNumber > 1)
                {
                    adjustedRowSpacing = (heightSize - measuredHeight) / (rowNumber - 1);
                }
                else
                {
                    adjustedRowSpacing = 0;
                }
                measuredHeight = heightSize;
            }
            else
            {
                adjustedRowSpacing = rowSpacing;
                if (rowNumber > 1)
                {
                    measuredHeight = heightMode == MeasureSpecMode.Unspecified ?
                                       ((int)(measuredHeight + adjustedRowSpacing * (rowNumber - 1)))
                                       : (Math.Min((int)(measuredHeight + adjustedRowSpacing * (rowNumber - 1)), heightSize));
                }
            }

            exactMeasuredHeight = measuredHeight;

            measuredWidth = widthMode
                == MeasureSpecMode.Exactly ? widthSize : measuredWidth;
            measuredHeight = heightMode
                == MeasureSpecMode.Exactly ? heightSize : measuredHeight;

            SetMeasuredDimension(measuredWidth, measuredHeight);
        }

        override protected void OnLayout(bool changed, int l, int t, int r, int b)
        {
            int paddingLeft = PaddingLeft;
            int paddingRight = PaddingRight;
            int paddingTop = PaddingTop;
            int paddingBottom = PaddingBottom;

            int x = paddingLeft;
            int y = paddingTop;

            int horizontalGravity = Gravity & (int)GravityFlags.HorizontalGravityMask;
            int verticalGravity = Gravity & (int)GravityFlags.VerticalGravityMask;

            int horizontalPadding = paddingLeft + paddingRight;
            int layoutWidth = r - l;
            x += HorizontalGravityOffsetForRow(
                horizontalGravity,
                layoutWidth,
                horizontalPadding,
                0);

            int rowCount = childNumberForRow.Count;
            int childIndex = 0;
            for (int row = 0; row < rowCount; row++)
            {
                int childNum = childNumberForRow.ElementAt(row);
                int rowHeight = heightForRow.ElementAt(row);
                int spacing = horizontalSpacingForRow.ElementAt(row);

                for (int i = 0; i < childNum && childIndex < ChildCount;)
                {
                    var child = GetChildAt(childIndex++);
                    if (child.Visibility == ViewStates.Gone)
                    {
                        continue;
                    }
                    else
                    {
                        i++;
                    }

                    LayoutParams childParams = child.LayoutParameters;
                    int marginLeft = 0;
                    int marginTop = 0;
                    int marginRight = 0;

                    if (childParams is MarginLayoutParams)
                    {
                        MarginLayoutParams marginParams = (MarginLayoutParams)childParams;
                        marginLeft = marginParams.LeftMargin;
                        marginRight = marginParams.RightMargin;
                        marginTop = marginParams.TopMargin;
                    }

                    int childWidth = child.MeasuredWidth;
                    int childHeight = child.MeasuredHeight;
                    child.Layout(
                        x + marginLeft,
                        y + marginTop,
                        x + marginLeft + childWidth,
                        y + marginTop + childHeight);
                    x += childWidth + spacing + marginLeft + marginRight;
                }
                x = paddingLeft;
                x += HorizontalGravityOffsetForRow(
                    horizontalGravity,
                    layoutWidth,
                    horizontalPadding,
                    row + 1);
                y += rowHeight + adjustedRowSpacing;
            }
        }

        override protected LayoutParams GenerateLayoutParams(LayoutParams p)
        {
            return new MarginLayoutParams(p);
        }

        override public LayoutParams GenerateLayoutParams(IAttributeSet attrs)
        {
            return new MarginLayoutParams(Context, attrs);
        }

        override public bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return false;
        }

        /*
         * Creates TagView with provided text
         */
        private TagView CreateTagView(string text)
        {
            TagView textView = new TagView(Context);
            textView.Text = text;

            return textView;
        }

        /*
         * Toogles the current selection state of a child TagView
         */
        private void ToggleSelection(TagView tagView)
        {
            tagView.SetSelected(tagView.IsSelected ? false : true);
        }

        /*
         * Removes the child TagView from FlowLayout if RemoveSelection is true
         */
        private void RemoveTag(TagView tagView)
        {
            RemoveView(tagView);
        }

        /*
         * Converts a value provided in DP to PX value
         */
        private float DpToPx(float dp)
        {
            return TypedValue.ApplyDimension(
                ComplexUnitType.Dip,
                dp,
                Resources.DisplayMetrics);
        }

        /*
         * Determines the space for a necessary for a row prodiding the number of childs
         */
        private int SpacingForRow(
            int spacingAttribute,
            int rowSize,
            int usedSize,
            int childNum)
        {
            int spacing;
            if (spacingAttribute == SPACING_AUTO)
            {
                if (childNum > 1)
                {
                    spacing = (rowSize - usedSize) / (childNum - 1);
                }
                else
                {
                    spacing = 0;
                }
            }
            else
            {
                spacing = spacingAttribute;
            }
            return spacing;
        }

        /*
         * Determines left / right offset for horizontal gravity change
         */
        private int HorizontalGravityOffsetForRow(
            int horizontalGravity,
            int parentWidth,
            int horizontalPadding,
            int row)
        {
            int offset = 0;
            if (ChildSpacing == SPACING_AUTO
                || row >= widthForRow.Count
                || row >= childNumberForRow.Count
                || childNumberForRow.ElementAt(row) <= 0)
            {
                return 0;
            }

            switch (horizontalGravity)
            {
                case (int)GravityFlags.CenterHorizontal:
                    offset = (parentWidth - horizontalPadding - widthForRow.ElementAt(row)) / 2;
                    break;
                case (int)GravityFlags.Right:
                    offset = parentWidth - horizontalPadding - widthForRow.ElementAt(row);
                    break;
            }
            return offset;
        }

        /*
         * Creates and adds child TagViews for provided data
         */
        public void SetData(List<string> items)
        {
            foreach (string text in items)
            {
                AddView(CreateTagView(text));
            }
        }

        /*
         * Expands the FlowLayout to it's max height
         */
        public void Expand()
        {
            SetMaxRows(int.MaxValue);
        }

        /*
         * Collapses the FlowLayout to the provided number of rows
         */
        public void Collapse(int minRows)
        {
            SetMaxRows(minRows);
        }

        /*
         * Sets TagSelectionHandler to handle child clicks and notify back to Activity
         */
        public void SetTagSelectionHandler(ITagSelection tagSelectionHandler)
        {
            this.TagSelectionHandler = tagSelectionHandler;
            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChildAt(i);
                child.Id = i;
                child.Click += ChildClickHandler;
            }
        }

        /*
         * Handles click on a child TagView. Toggles selection or removes if necessary
         */
        void ChildClickHandler(object sender, EventArgs args)
        {
            if (sender is TagView)
            {
                TagView tagView = (TagView)sender;
                int position = tagView.Id;
                string text = tagView.Text;

                if (TagSelectionHandler != null)
                {
                    TagSelectionHandler.onTagSelected(position, tagView, this);
                }

                if (RemoveSelection)
                {
                    RemoveTag(tagView);
                }
                else
                {
                    ToggleSelection(tagView);
                }
            }
        }
    }
}
