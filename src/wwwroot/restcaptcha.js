/*!
 * Copyright (C) 2025 STÜBER SYSTEMS GmbH
 *
 * This file is part of the RESTCaptcha project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with this program. If not, see <https://www.gnu.org/licenses/>.
 */

/**
 * Represents a specific failure when attempting to solve a CAPTCHA challenge.
 * 
 * This exception is thrown when the CAPTCHA cannot be solved within reasonable
 * constraints (e.g. max number of attempts or time limits). It is **not** used for
 * unexpected runtime errors (such as network or API issues).
 */
class CaptchaFailedException extends Error {

    /**
     * Constructs a new CaptchaFailedException instance.
     *
     * @param {string} message - A human-readable description of the failure.
     */
    constructor(message) {
        super(message);
        this.name = "CaptchaFailedException";
    }
}

/**
 * This class represents the client-side of the CAPTCHA mechanism.
 * 
 * It fetches a challenge from the backend, solves a hash-based puzzle, 
 * and injects hidden input fields into a form for server-side validation.
 * 
 * Event handlers like `onStarted`, `onSolved`, `onFailed`, and `onError` 
 * can be set to control UI behavior during the solving process.
 */
class RestCaptcha {

    /**
     * Constructs a new RestCaptcha instance.
     *
     * @param {string} challengeUrl - The URL of the RESTCaptcha challenge endpoint.
     * @param {string} formId - The ID of the HTML form to inject CAPTCHA solution into (defaults to 'restcaptcha-form').
     */
    constructor(challengeUrl, formId = "restcaptcha-form") {

        // Get and store form element
        this._form = document.getElementById(formId);
        if (!this._form) {
            console.warn("RESTCaptcha", 'Form element with ID "${formId}" was not found.');
        }

        this._challengeUrl = challengeUrl;
        this._onStarted = null;
        this._onSolved = null;
        this._onFailed = null;
        this._onError = null;
    }

    /**
     * Gets the currently assigned onStarted handler.
     * @returns {Function|null}
     */
    get onStarted() {
        return this._onStarted;
    }

    /**
     * Sets the onStarted handler.
     * Must be a function or null; throws otherwise.
     * @param {Function|null} handler
     */
    set onStarted(handler) {
        if (typeof handler === "function" || handler === null) {
            this._onStarted = handler;
        } else {
            throw new TypeError("onStarted must be a function or null.");
        }
    }

    /**
     * Gets the currently assigned onSolved handler.
     * @returns {Function|null}
     */
    get onSolved() {
        return this._onStarted;
    }

    /**
     * Sets the onSolved handler.
     * Must be a function or null; throws otherwise.
     * @param {Function|null} handler
     */
    set onSolved(handler) {
        if (typeof handler === "function" || handler === null) {
            this._onSolved = handler;
        } else {
            throw new TypeError("onSolved must be a function or null.");
        }
    }

    /**
     * Gets the currently assigned onFailed handler.
     * @returns {Function|null}
     */
    get onFailed() {
        return this._onStarted;
    }

    /**
     * Sets the onFailed handler.
     * Must be a function or null; throws otherwise.
     * @param {Function|null} handler
     */
    set onFailed(handler) {
        if (typeof handler === "function" || handler === null) {
            this._onFailed = handler;
        } else {
            throw new TypeError("onFailed must be a function or null.");
        }
    }

    /**
     * Gets the currently assigned onError handler.
     * @returns {Function|null}
     */
    get onError() {
        return this._onError;
    }

    /**
     * Sets the onError handler.
     * Must be a function or null; throws otherwise.
     * @param {Function|null} handler
     */
    set onError(handler) {
        if (typeof handler === "function" || handler === null) {
            this._onError = handler;
        } else {
            throw new TypeError("onError must be a function or null.");
        }
    }

    /**
     * Solves the proof-of-work challenge and injects the result into the form.
     */
    async solve() {
        if (this._form) return;
        try {
            if (this._onStarted) {
                this._onStarted();
            }

            const challengeRes = await fetch(this._challengeUrl, { method: "GET" });
            if (!challengeRes.ok) {
                throw new Error(`Challenge fetch failed: ${challengeRes.status} ${challengeRes.statusText}`);
            }

            const { nonce, nonceSignature, timeStamp, proofOfWorkDifficulty } = await challengeRes.json();

            let solution;
            let attempts = 0;
            while (true) {
                solution = Math.random().toString(36).substring(2, 10);
                const input = `${nonce}:${solution}`;
                const data = new TextEncoder().encode(input);
                const digest = await crypto.subtle.digest("SHA-256", data);
                const hex = [...new Uint8Array(digest)].map(b => b.toString(16).padStart(2, "0")).join("");
                attempts++;
                if (hex.startsWith("0".repeat(proofOfWorkDifficulty))) break;
                if (attempts > 100_000) {
                    throw new CaptchaFailedException("Too many attempts while solving CAPTCHA");
                }
            }

            const fingerprint = await this.getFingerprintHash();

            this._form.insertAdjacentHTML("beforeend", `
                <input type="hidden" name="nonce" value="${nonce}">
                <input type="hidden" name="nonceSignature" value="${nonceSignature}">
                <input type="hidden" name="timeStamp" value="${timeStamp}">
                <input type="hidden" name="solution" value="${solution}">
                <input type="hidden" name="fingerprint" value="${fingerprint}">
            `);

            if (this._onSolved) {
                this._onSolved();
            }

        } catch (err) {
            if (err instanceof CaptchaFailedException) {
                if (this._onFailed) {
                    this._onFailed(err.message || "CAPTCHA failed");
                }
            } else {
                if (this._onError) {
                    this._onError(err);
                } else {
                    console.error("RESTCaptcha:", err);
                }
            }
        }
    }

    /**
     * Generates a SHA-256 hash representing the user's device/browser environment.
     * @returns {Promise<string>} A hexadecimal fingerprint hash.
     */
    async getFingerprintHash() {
        const fingerprintParts = {
            userAgent: navigator.userAgent || "",
            language: navigator.language || "",
            width: screen.width,
            height: screen.height,
            colorDepth: screen.colorDepth,
            concurrency: navigator.hardwareConcurrency || "",
            memory: navigator.deviceMemory || "",
            chrome: !!window.chrome,
            webdriver: navigator.webdriver,
            timezone: Intl.DateTimeFormat().resolvedOptions().timeZone || ""
        };

        const raw = Object.values(fingerprintParts).join("|");
        const data = new TextEncoder().encode(raw);
        const digest = await crypto.subtle.digest("SHA-256", data);

        return [...new Uint8Array(digest)].map(b => b.toString(16).padStart(2, "0")).join("");
    }
}
