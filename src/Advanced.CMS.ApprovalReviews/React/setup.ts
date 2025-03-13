import { afterAll, beforeAll, vi } from "vitest";

const originalError = console.error;
beforeAll(() => {
    vi.spyOn(console, "error").mockImplementation((...args) => {
        if (typeof args[0] === "string" && args[0].includes("Warning")) {
            return;
        }
        return originalError.call(console, args);
    });
});

afterAll(() => {
    // @ts-ignore
    console.error.mockReset();
});
