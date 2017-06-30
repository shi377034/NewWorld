//-----------------------------------------------------------------------
// <copyright file="RectExtensions.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities
{
    using UnityEngine;

    public static class RectExtensions
    {
        public static Rect SetWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        public static Rect SetHeight(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

		public static Rect SetSize(this Rect rect, float width, float height)
		{
			rect.width = width;
			rect.height = height;
			return rect;
		}
		
		public static Rect SetSize(this Rect rect, Vector2 size)
		{
			rect.size = size;
			return rect;
		}

        public static Rect HorizontalPadding(this Rect rect, float padding)
        {
            rect.x += padding;
            rect.width -= padding * 2f;
            return rect;
        }

        public static Rect HorizontalPadding(this Rect rect, float left, float right)
        {
            rect.x += left;
            rect.width -= (left + right);
            return rect;
        }

        public static Rect VerticalPadding(this Rect rect, float padding)
        {
            rect.y += padding;
            rect.height -= padding * 2f;
            return rect;
        }

        public static Rect VerticalPadding(this Rect rect, float top, float bottom)
        {
            rect.y += top;
            rect.height -= (top + bottom);
            return rect;
        }

		public static Rect Padding(this Rect rect, float padding)
		{
			rect.position += new Vector2(padding, padding);
			rect.size -= new Vector2(padding, padding) * 2f;
			return rect;
		}

        public static Rect Padding(this Rect rect, float horizontal, float vertical)
        {
            rect.position += new Vector2(horizontal, vertical);
            rect.size -= new Vector2(horizontal, vertical) * 2f;
            return rect;
        }

        public static Rect Padding(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.position += new Vector2(left, top);
            rect.size -= new Vector2(left + right, top + bottom);
            return rect;
        }

        public static Rect AlignLeft(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        public static Rect AlignCenter(this Rect rect, float width)
        {
            rect.x = rect.width * 0.5f - width * 0.5f;
            rect.width = width;
            return rect;
        }

        public static Rect AlignRight(this Rect rect, float width)
        {
            rect.x = rect.x + rect.width - width;
            rect.width = width;
            return rect;
        }

        public static Rect AlignTop(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

        public static Rect AlignMiddle(this Rect rect, float height)
        {
            rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
            rect.height = height;
            return rect;
        }

        public static Rect AlignBottom(this Rect rect, float height)
        {
            rect.y = rect.y + rect.height - height;
            rect.height = height;
            return rect;
        }

        //public static Rect AlignLeftIn(this Rect rect, Rect area)
        //{
        //}
        //public static Rect AlignCenterIn(this Rect rect, Rect area)
        //{
        //}
        //public static Rect AlignRightIn(this Rect rect, Rect area)
        //{
        //}
        //public static Rect AlignTopIn(this Rect rect, Rect area)
        //{
        //}
        //public static Rect AlignMiddleIn(this Rect rect, Rect area)
        //{
        //}
        //public static Rect AlignBottomIn(this Rect rect, Rect area)
        //{
        //}

		public static Rect Expand(this Rect rect, float expand)
		{
			rect.position -= new Vector2(expand, expand);
			rect.size += new Vector2(expand, expand) * 2f;
			return rect;
		}
		
		public static Rect Expand(this Rect rect, float horizontal, float vertical)
		{
			rect.position -= new Vector2(horizontal, vertical);
			rect.size += new Vector2(horizontal, vertical) * 2f;
			return rect;
		}

		public static Rect Expand(this Rect rect, float left, float right, float top, float bottom)
		{
			rect.position -= new Vector2(left, top);
			rect.size += new Vector2(left + right, top + bottom);
			return rect;
		}

		public static Rect Split(this Rect rect, int index, int count)
		{
			int width = (int)(rect.width / count);
			rect.width = width;
			rect.x += width * index;
			return rect;
		}

		public static Rect SetCenterX(this Rect rect, float x)
		{
			rect.center = new Vector2(x, rect.center.y);
			return rect;
		}

		public static Rect SetCenterY(this Rect rect, float y)
		{
			rect.center = new Vector2(rect.center.x, y);
			return rect;
		}

		public static Rect SetCenter(this Rect rect, float x, float y)
		{
			rect.center = new Vector2(x, y);
			return rect;
		}

        public static Rect SetCenter(this Rect rect, Vector2 center)
        {
            rect.center = center;
            return rect;
        }

        public static Rect SetPosition(this Rect rect, Vector2 position)
        {
            rect.position = position;
            return rect;
        }

        public static Rect ResetPosition(this Rect rect)
        {
            rect.position = Vector2.zero;
            return rect;
        }

        public static Rect AddPosition(this Rect rect, Vector2 move)
        {
            rect.position += move;
            return rect;
        }

        public static Rect MoveHorizontal(this Rect rect, float distance)
        {
            rect.x += distance;
            return rect;
        }

        public static Rect MoveVertical(this Rect rect, float distance)
        {
            rect.y += distance;
            return rect;
        }

        public static Rect SetMin(this Rect rect, Vector2 min)
        {
            rect.min = min;
            return rect;
        }

        public static Rect AddMin(this Rect rect, Vector2 value)
        {
            rect.min += value;
            return rect;
        }

        public static Rect SubMin(this Rect rect, Vector2 value)
        {
            rect.min -= value;
            return rect;
        }

        public static Rect SetMax(this Rect rect, Vector2 max)
        {
            rect.max = max;
            return rect;
        }

        public static Rect AddMax(this Rect rect, Vector2 value)
        {
            rect.max += value;
            return rect;
        }

        public static Rect SubMax(this Rect rect, Vector2 value)
        {
            rect.max -= value;
            return rect;
        }

        public static Rect SetXMin(this Rect rect, float xMin)
        {
            rect.xMin = xMin;
            return rect;
        }

        public static Rect AddXMin(this Rect rect, float value)
        {
            rect.xMin += value;
            return rect;
        }

        public static Rect SubXMin(this Rect rect, float value)
        {
            rect.xMin -= value;
            return rect;
        }

        public static Rect SetXMax(this Rect rect, float xMax)
        {
            rect.xMax = xMax;
            return rect;
        }

        public static Rect AddXMax(this Rect rect, float value)
        {
            rect.xMax += value;
            return rect;
        }

        public static Rect SubXMax(this Rect rect, float value)
        {
            rect.xMax -= value;
            return rect;
        }

        public static Rect SetYMin(this Rect rect, float yMin)
        {
            rect.yMin = yMin;
            return rect;
        }

        public static Rect AddYMin(this Rect rect, float value)
        {
            rect.yMin += value;
            return rect;
        }

        public static Rect SubYMin(this Rect rect, float value)
        {
            rect.yMin -= value;
            return rect;
        }

        public static Rect SetYMax(this Rect rect, float yMax)
        {
            rect.yMax = yMax;
            return rect;
        }

        public static Rect AddYMax(this Rect rect, float value)
        {
            rect.yMax += value;
            return rect;
        }

        public static Rect SubYMax(this Rect rect, float value)
        {
            rect.yMax -= value;
            return rect;
        }

		public static Rect MinWidth(this Rect rect, float minWidth)
		{
			rect.width = Mathf.Max(rect.width, minWidth);
			return rect;
		}

        //public static Rect ScaleFromCenter(this Rect rect, float scale)
        //{
        //}

        //public static Rect ClipTo(this Rect rect, Rect clip)
        //{
        //}
    }
}