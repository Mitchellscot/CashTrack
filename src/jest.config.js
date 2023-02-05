/** @type {import('ts-jest').JestConfigWithTsJest} */
const config = {
	preset: 'ts-jest',
	testEnvironment: 'jest-environment-jsdom',
	roots: ['Scripts/_tests']
};
module.exports = config;