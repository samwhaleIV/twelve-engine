using System;
using System.Collections.Generic;
using System.Linq;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI {
    internal static class RenderElementSorter {

        /* ------------------------------------------------------------------------------------------------------
           Tightly coupled sorting logic, this matches the rendering state to the input state. Don't fuck with it */
        private static RenderElement[] getElementsCache(List<RenderElement> elements) {
            return elements.OrderByDescending(element => element.Depth).ToArray();
        }
        private static RenderElement[] getScrollCache(List<RenderElement> elements) {
            return elements.Where(element => element.IsScrollable).Reverse().OrderBy(element => element.Depth).ToArray();
        }
        private static RenderElement[] getInteractionCache(List<RenderElement> elements) {
            /* OrderByDescending is not used in order to preserve the implicit z-indexing of same layer and addChild call orders */
            return elements.Where(element => element.IsInteractable).Reverse().OrderBy(element => element.Depth).ToArray();
        }
        private static void calculateDepths(List<RenderElement> elements,float maxDepth) {
            foreach(RenderElement element in elements) {
                element.Depth = 1f - element.Depth / maxDepth / 1f;
            }
        }
        private static RenderFrame[] getFrameCache(RenderElement[] elements) {
            var queue = new Queue<RenderFrame>();
            foreach(var element in elements) {
                if(element is RenderFrame frame) {
                    queue.Enqueue(frame);
                }
            }
            return queue.ToArray();
        }
        /* ------------------------------------------------------------------------------------------------------ */

        private static void getAllChildren(
            Element source,List<RenderElement> elements,int currentDepth,ref int maxDepth
        ) {
            currentDepth += 1;
            if(currentDepth > maxDepth) {
                maxDepth = currentDepth;
            }

            if(source is RenderElement renderElement) {
                elements.Add(renderElement);
                renderElement.Depth = currentDepth;
            }

            if(source is RenderFrame) {
                return;
            }
            foreach(var child in source.Children) {
                getAllChildren(child,elements,currentDepth,ref maxDepth);
            }
        }
        private static void getAllChildren(Element rootNode,List<RenderElement> elements,out int depth) {
            int maxDepth = 1;
            foreach(var child in rootNode.Children) {
                getAllChildren(child,elements,-1,ref maxDepth);
            }
            depth = Math.Max(maxDepth,1);
        }

        private static (List<RenderElement> elements, int maxDepth) getChildrenAndDepth(Element rootNode) {
            var elements = new List<RenderElement>();
            getAllChildren(rootNode,elements,out int maxDepth);
            return (elements, maxDepth);
        }

        public static RenderCache GenerateCache(Element rootNode) {
            (List<RenderElement> elements, int maxDepth) = getChildrenAndDepth(rootNode);
            calculateDepths(elements,maxDepth);

            var sortedElements = getElementsCache(elements);

            var frames = getFrameCache(sortedElements);

            var interact = getInteractionCache(elements);
            var scroll = getScrollCache(elements);

            return new RenderCache(sortedElements,interact,scroll,frames);
        }
    }
}
