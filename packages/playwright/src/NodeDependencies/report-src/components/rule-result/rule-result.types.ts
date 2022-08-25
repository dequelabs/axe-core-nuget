import { classNamesFunction, IStyle, IStyleFunctionOrObject, ITheme } from "@fluentui/react";
import { Result } from "axe-core";

interface IRuleResultProps {
    styles?: IStyleFunctionOrObject<IRuleResultStyleProps, IRuleResultStyles>;
    result: Result;
    index: number;
}

interface IRuleResultStyleProps {
    theme: ITheme;
    index: number;
}

interface IRuleResultStyles {
    ruleName?: IStyle;
    root?: IStyle;
}

const getClassNames = classNamesFunction<IRuleResultStyleProps, IRuleResultStyles>();

export { IRuleResultProps, IRuleResultStyleProps, IRuleResultStyles, getClassNames };