import pluginVue from 'eslint-plugin-vue'
import vueTsEslintConfig from '@vue/eslint-config-typescript'
import skipFormatting from '@vue/eslint-config-prettier/skip-formatting'
import typeScriptEsLintPlugin from '@typescript-eslint/eslint-plugin'
import typeScriptEsLintParser from '@typescript-eslint/parser'
import esLintConfigPrettier from '@vue/eslint-config-prettier'

module.exports = [
  'eslint:recommended',
  typeScriptEsLintPlugin.configs.recommended,
  esLintConfigPrettier,
  {
    name: 'app/files-to-lint',
    languageOptions: {
      parser: typeScriptEsLintParser
    },
    files: ['**/*.{ts,mts,tsx,vue}'],
    rules: {
      '@typescript-eslint/array-type': 'error',
      'no-unused-vars': 'off',
      '@typescript-eslint/no-unused-vars': 'off',
      '@typescript-eslint/no-explicit-any': 'warn'
    }
  },
  {
    name: 'app/files-to-ignore',
    ignores: ['**/dist/**', '**/dist-ssr/**', '**/coverage/**']
  },

  ...pluginVue.configs['flat/essential'],
  skipFormatting
]
