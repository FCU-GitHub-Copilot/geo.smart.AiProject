import { dark as blueDark, light as blueLight } from './blue';
import { dark as brownDark, light as brownLight } from './brown';
import { dark as defaultDark, light as defaultLight } from './default';
import { dark as greenDark, light as greenLight } from './green';
import { dark as orangeDark, light as orangeLight } from './orange';
import { dark as pinkDark, light as pinkLight } from './pink';
import { dark as purpleDark, light as purpleLight } from './purple';
import { dark as redDark, light as redLight } from './red';
import { dark as yellowDark, light as yellowLight } from './yellow';

export const themes = {
    defaultLight,
    defaultDark,
    purpleLight,
    purpleDark,
    brownLight,
    brownDark,
    orangeLight,
    orangeDark,
    greenLight,
    greenDark,
    pinkLight,
    pinkDark,
    blueLight,
    blueDark,
    yellowLight,
    yellowDark,
    redLight,
    redDark,
} as const;

export type ThemesKey = keyof typeof themes;
