
import { Config } from "jest";

const config: Config = {
    rootDir: "./report-src",
    transform: {
        "^.+\\.(t|j)sx?$": "@swc/jest",
    },
    testEnvironment: "jsdom",
    verbose: true,
    setupFilesAfterEnv: ["<rootDir>/../setupTests.ts"]
}

export default config;