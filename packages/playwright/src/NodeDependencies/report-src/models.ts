import type { Result } from "axe-core";

type AxeResults = {
    url: string;
    timestamp: string;
    passes: Result[];
    violations: Result[];
    incomplete: Result[];
    inapplicable: Result[];
}

export { AxeResults }