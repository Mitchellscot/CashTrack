import 'bootstrap/dist/js/bootstrap.min.js';
import {
	hideLoadingSpinner,
	activateSpinnerOnClick,
} from '../Utility/loading-spinner';
import getToastMessages from '../Utility/toast-messages';
import initializeTooltips from '../Utility/initialize-tooltips';
import { Button } from 'bootstrap';

initializeTooltips();
hideLoadingSpinner();
activateSpinnerOnClick();
getToastMessages();

(() => {
    'use strict'

    const getStoredTheme = () => localStorage.getItem('theme')
    const setStoredTheme = (theme: string | null) => localStorage.setItem('theme', theme!)

    const getPreferredTheme = () => {
        const storedTheme = getStoredTheme()
        if (storedTheme) {
            return storedTheme
        }

        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
    }

    const setTheme = (theme: string | null) => {
        if (theme === 'auto' && window.matchMedia('(prefers-color-scheme: dark)').matches) {
            document.documentElement.setAttribute('data-bs-theme', 'dark')
        } else {
            document.documentElement.setAttribute('data-bs-theme', theme!)
        }
    }

    setTheme(getPreferredTheme())

    const showActiveTheme = (theme: string | null, focus: boolean = false) => {
        const themeSwitcher = document.querySelector('#bd-theme') as HTMLButtonElement;

        if (!themeSwitcher) {
            return
        }

        const themeSwitcherText = document.querySelector('#bd-theme-text')
        const activeThemeIcon = document.querySelector('.theme-icon-active use')
        const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`) as HTMLButtonElement
        const svgOfActiveBtn = btnToActive!.querySelector('svg use')!.getAttribute('href')

        document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
            element.classList.remove('active')
            element.setAttribute('aria-pressed', 'false')
        })

        btnToActive!.classList.add('active')
        btnToActive!.setAttribute('aria-pressed', 'true')
        activeThemeIcon!.setAttribute('href', svgOfActiveBtn!)
        const themeSwitcherLabel = `${themeSwitcherText!.textContent} (${btnToActive!.dataset.bsThemeValue})`
        themeSwitcher.setAttribute('aria-label', themeSwitcherLabel)

        if (focus) {
            themeSwitcher.focus()
        }
    }

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', () => {
        const storedTheme = getStoredTheme()
        if (storedTheme !== 'light' && storedTheme !== 'dark') {
            setTheme(getPreferredTheme())
        }
    })

    window.addEventListener('DOMContentLoaded', () => {
        showActiveTheme(getPreferredTheme())

        document.querySelectorAll('[data-bs-theme-value]')
            .forEach(toggle => {
                toggle.addEventListener('click', () => {
                    const theme = toggle.getAttribute('data-bs-theme-value')
                    setStoredTheme(theme)
                    setTheme(theme)
                    showActiveTheme(theme, true)
                })
            })
    })
})()