import { classNamesFunction, IStyle, IStyleFunctionOrObject, ITheme } from "@fluentui/react";
import type { Result } from "axe-core";

interface IRuleListProps {
    styles?: IStyleFunctionOrObject<IRuleListStyleProps, IRuleListStyles>;
    rules: Result[];
    header: string;
}

interface IRuleListStyleProps {
    theme: ITheme;
}

interface IRuleListStyles {
    ruleListHeader?: IStyle;
}

const getClassNames = classNamesFunction<IRuleListStyleProps, IRuleListStyles>();

export { IRuleListProps, IRuleListStyleProps, IRuleListStyles, getClassNames }