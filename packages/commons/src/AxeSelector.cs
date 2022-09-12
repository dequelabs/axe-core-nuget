using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deque.AxeCore.Commons
{
    /// <summary>
    /// Represents a CSS selector pointing to an element (or elements) on a page. Also supports elements which are nested inside of
    /// iframes and/or open shadow DOMs by using nested lists of selectors representing the chain of iframe/shadow root elements
    /// leading up to the final selector (see <see cref="FrameSelectors"/> and <see cref="FrameShadowSelectors"/>).
    /// 
    /// Most users just interested in writing test cases should prefer using <see cref="ToString"/> rather than interacting with
    /// the individual properties of this class.
    /// </summary>
    /// <example><![CDATA[
    /// var simpleSelector = new AxeSelector("#some-element-id");
    /// 
    /// // simpleSelector.Selector == "#some-element-id"
    /// // simpleSelector.FrameSelectors == ["#some-element-id"]
    /// // simpleSelector.FrameShadowSelectors = [["#some-element-id"]]
    /// // simpleSelector.ToString() == "#some-element-id"
    /// ]]></example>
    /// <example><![CDATA[
    /// var selectorInIframe = new AxeSelector("#child-element", new List<string> { "#parent-iframe-element" });
    /// 
    /// // selectorInIframe.Selector throws InvalidOperationException
    /// // selectorInIframe.FrameSelectors == ["#parent-iframe-element", "#child-element"]
    /// // selectorInIframe.FrameShadowSelectors == [["#parent-iframe-element"], ["#child-element"]]
    /// // selectorInIframe.ToString() == "[\"#parent-iframe-element\", \"#child-element\"]" 
    /// ]]></example>
    /// <example><![CDATA[
    /// var selectorInShadowDomInIframe = AxeSelector.FromFrameShadowSelectors(new List<List<string>>
    /// {
    ///     new List<string> { "#parent-iframe-element" },
    ///     new List<string> { "#shadow-root-in-iframe", "#child-in-shadow-root" }
    /// });
    /// 
    /// // selectorInShadowDomInIframe.Selector throws InvalidOperationException
    /// // selectorInShadowDomInIframe.FrameSelectors throws InvalidOperationException
    /// // selectorInShadowDomInIframe.FrameShadowSelectors == [["#parent-iframe-element"], ["#shadow-root-in-iframe", "#child-in-shadow-root"]]
    /// // selectorInShadowDomInIframe.ToString() == "[\"#parent-iframe-element\", [\"#shadow-root-in-iframe\", \"#child-element\"]]" 
    /// ]]></example>
    [JsonConverter(typeof(AxeSelectorJsonConverter))]
    public class AxeSelector : IEquatable<AxeSelector>
    {
        /// <summary>
        /// For simple selectors which do not require traversing iframes or shadow DOMs, this property can be used as a shortcut
        /// to access the CSS selector directly. For a string representation of the selector which will work regardless of iframe
        /// and shadow DOM usage, see <see cref="ToString"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the AxeSelector requires traversing iframes and/or shadow DOMs</exception>
        public string Selector {
            get {
                if (FrameShadowSelectors.Count > 1) {
                    throw new InvalidOperationException($"AxeSelector {this} represents an element nested within iframe(s); it cannot be represented as a single Selector. Use FrameSelectors or FrameShadowSelectors instead.");
                }
                List<string> shadowSelectors = FrameShadowSelectors[0];
                if (shadowSelectors.Count > 1) {
                    throw new InvalidOperationException($"AxeSelector {this} represents an element nested within a shadow DOM; it cannot be represented as a single Selector. Use FrameShadowSelectors instead.");
                }
                return shadowSelectors[0];
            }
        }

        /// <summary>
        /// For selectors which may involve traversing iframes but do not involve traversing shadow DOMs, this property can be
        /// used as a shortcut to access the selectors for the chain of iframe elements leading up to the final selector.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the AxeSelector requires traversing shadow DOM(s)</exception>
        public IEnumerable<string> FrameSelectors {
            get {
                if (FrameShadowSelectors.Any(shadowSelectors => shadowSelectors.Count > 1)) {
                    throw new InvalidOperationException($"AxeSelector {this} represents an element nested within a shadow DOM; it cannot be represented with only one selector per frame. Use FrameShadowSelectors instead.");
                }
                return FrameShadowSelectors.Select(shadowSelectors => shadowSelectors[0]);
            }
        }

        /// <summary>
        /// Each element of this list represents one "shadow selector list" per frame leading up to the element in question,
        /// where a "shadow selector list" is a list of selectors each representing one shadow root element in a chain of
        /// shadow DOMs leading up to a given iframe/element.
        /// 
        /// For a simple AxeSelector which does not involve iframes or shadow DOMs, this will be a singleton list containing
        /// a singleton list containing the same value as <see cref="Selector"/>.
        /// </summary>
        /// <example>
        ///   * If a page contains an element #a
        ///   * ...which has an attached shadow DOM containing an iframe element #b
        ///   * ...whose content window contains another iframe element #c
        ///   * ...whose content window contains an element #d
        ///   * ...which has an attached shadow DOM containing element #e
        ///  
        /// ...then FrameShadowSelectors for an AxeSelector representing #e would be structured like [["#a", "#b"], ["#c"], ["#d", "#e"]].
        /// </example>
        /// <remarks>
        /// It is invalid for either the outer list or any inner list to be empty.
        /// </remarks>
        public List<List<string>> FrameShadowSelectors { get; }

        /// <summary>
        /// Constructs an AxeSelector which represents an element in the topmost frame of a page and which does not involve any shadow DOMs.
        /// </summary>
        /// <param name="selector"></param>
        public AxeSelector(string selector)
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            FrameShadowSelectors = new List<List<string>> { new List<string> { selector } };
        }

        /// <summary>
        /// Constructs an AxeSelector which represents an element in any frame of a page, but which does not involve any shadow DOMs.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="ancestorFrames"></param>
        public AxeSelector(string selector, List<string> ancestorFrames)
        {
            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            if (ancestorFrames is null)
            {
                throw new ArgumentNullException(nameof(ancestorFrames));
            }

            if (ancestorFrames.Count == 0)
            {
                throw new ArgumentException($"{nameof(ancestorFrames)} must be non-empty");
            }

            FrameShadowSelectors = ancestorFrames.Select(frameSelector => new List<string> { frameSelector }).ToList();
            FrameShadowSelectors.Add(new List<string> { selector });
        }

        /// <summary>
        /// Constructs an AxeSelector which represents an element that requires shadow DOM traversal to represent.
        /// </summary>
        /// <param name="frameShadowSelectors">See <see cref="FrameShadowSelectors"/>.</param>
        /// <returns>An AxeSelector wrapping <paramref name="frameShadowSelectors"/></returns>
        public static AxeSelector FromFrameShadowSelectors(List<List<string>> frameShadowSelectors) => new AxeSelector(frameShadowSelectors);

        // This constructor is intentionally hidden behind FromFrameShadowSelectors to make it harder to use by accident; in practice,
        // it's rarer for a user to actually need it than it is for a user to accidentally try to misuse it in an attempt to have one
        // AxeSelector represent multiple independent selectors.
        private AxeSelector(List<List<string>> frameShadowSelectors)
        {
            if (frameShadowSelectors is null)
            {
                throw new ArgumentNullException(nameof(frameShadowSelectors));
            }

            if (frameShadowSelectors.Count == 0)
            {
                throw new ArgumentException($"Argument must be non-empty: ", nameof(frameShadowSelectors));
            }

            if (frameShadowSelectors.Any(shadowSelectors => shadowSelectors is null || shadowSelectors.Count == 0))
            {
                throw new ArgumentException($"Argument elements must all be non-null and non-empty: ", nameof(frameShadowSelectors));
            }

            FrameShadowSelectors = frameShadowSelectors;
        }

        /// <summary>
        /// Returns a string which represents a minimal form of the AxeSelector, similar to how axe-core would represent the
        /// selector in the "target" property of node in an axe result object. Suitable for display in user-facing messages.
        /// </summary>
        /// <returns>A string representation of the AxeSelector</returns>
        public override string ToString()
        {
            if (FrameShadowSelectors.Count == 1 && FrameShadowSelectors[0].Count == 1) {
                return FrameShadowSelectors[0][0];
            } else {
                return $"[{string.Join(", ", FrameShadowSelectors.Select(ShadowSelectorsToString))}]";
            }
        }

        private static string ShadowSelectorsToString(List<string> shadowSelectors)
        {
            if (shadowSelectors.Count == 1)
            {
                return QuoteString(shadowSelectors[0]);
            }
            else
            {
                return $"[{string.Join(", ", shadowSelectors.Select(QuoteString))}]";
            }
        }

        private static string QuoteString(string unquotedString)
        {
            var quoteEscapedString = unquotedString.Replace("\\", "\\\\").Replace("\"", "\\\"");
            return $"\"{quoteEscapedString}\"";
        }

        /// <summary>
        /// Compares by-value to another object.
        /// </summary>
        /// <param name="other">The other object to compare to</param>
        /// <returns>Whether the two objects are both AxeSelectors which represent the same logical value</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AxeSelector);
        }

        /// <summary>
        /// Compares by-value to another AxeSelector.
        /// </summary>
        /// <param name="other">The other AxeSelector to compare to</param>
        /// <returns>Whether the two selectors represent the same logical value</returns>
        public bool Equals(AxeSelector other)
        {
            if (other is null) { return false; }

            if (Object.ReferenceEquals(this, other)) { return true; }

            // We want deep equality of a List-of-Lists with a corresponding HashCode. ToString() already serializes
            // to a format suitable for doing that easily and is performant-enough for the purposes of this library,
            // so we just reuse it rather than dealing with implementing nested EqualityComparators.
            return other.ToString().Equals(this.ToString(), StringComparison.Ordinal);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // The constant is random and arbitrary, it's just there so the hash code of the object
            // differs from the hash code of the string.
            return 1788502736 ^ this.ToString().GetHashCode();
        }

        // TODO: update selenium package to deal with new type
    }
}
