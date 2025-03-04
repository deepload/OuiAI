# Contributing to OuiAI

Thank you for your interest in contributing to OuiAI! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How to Contribute](#how-to-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Suggesting Enhancements](#suggesting-enhancements)
  - [Your First Code Contribution](#your-first-code-contribution)
  - [Pull Requests](#pull-requests)
- [Development Process](#development-process)
  - [Git Workflow](#git-workflow)
  - [Development Workflow](#development-workflow)
  - [Commit Messages](#commit-messages)
  - [Code Style](#code-style)
- [Testing Guidelines](#testing-guidelines)
- [Documentation](#documentation)
- [Community](#community)

## Code of Conduct

The OuiAI project is committed to fostering an open and welcoming environment. By participating, you are expected to uphold our [Code of Conduct](CODE_OF_CONDUCT.md).

## How to Contribute

### Reporting Bugs

Before creating bug reports, please check the issue tracker to see if the problem has already been reported. If it has and the issue is still open, add a comment to the existing issue instead of opening a new one.

When reporting a bug, please include as much information as possible:

1. **Title**: Clear and descriptive title
2. **Description**: Detailed description of the issue
3. **Steps to Reproduce**: Specific steps to reproduce the behavior
4. **Expected Behavior**: What you expected to happen
5. **Actual Behavior**: What actually happened
6. **Environment**: 
   - OS and version
   - Browser/client version
   - .NET version
   - Node.js version (if applicable)
   - Any other relevant environment information
7. **Screenshots**: If applicable, add screenshots to help explain the problem
8. **Additional Context**: Any other context about the problem

### Suggesting Enhancements

1. **Title**: Clear and descriptive title
2. **Description**: Detailed description of the enhancement
3. **Rationale**: Why this enhancement would be useful
4. **Possible Implementation**: If you have ideas about how to implement the enhancement
5. **Additional Context**: Any other context or screenshots about the enhancement request

### Your First Code Contribution

Unsure where to begin contributing to OuiAI? You can start by looking through the `good-first-issue` and `help-wanted` issues:

* [Good First Issues](https://github.com/yourusername/OuiAI/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22) - issues that should only require a few lines of code
* [Help Wanted Issues](https://github.com/yourusername/OuiAI/issues?q=is%3Aissue+is%3Aopen+label%3A%22help+wanted%22) - issues that might be a bit more involved

### Pull Requests

1. **Fork the Repository**: Fork the repository to your GitHub account
2. **Create a Branch**: Create a branch for your feature or bug fix
3. **Make Changes**: Make your changes in your branch
4. **Follow Coding Standards**: Ensure your code follows our [coding standards](coding-standards.md)
5. **Add Tests**: Add tests for your changes
6. **Update Documentation**: Update documentation as needed
7. **Submit Pull Request**: Submit a pull request to the main repository

## Development Process

### Git Workflow

We use a simplified version of GitFlow:

1. **main**: Production-ready code
2. **develop**: Next release development
3. **feature/***:  New features
4. **bugfix/***:  Bug fixes
5. **hotfix/***:  Urgent fixes for production

### Development Workflow

1. **Create Issue**: Create an issue describing the task
2. **Assign Issue**: Assign the issue to yourself or someone else
3. **Create Branch**: Create a branch from develop (for features/fixes) or main (for hotfixes)
4. **Develop**: Make your changes
5. **Test**: Run tests to ensure your changes work as expected
6. **Create Pull Request**: Submit a PR to merge your changes back to the source branch
7. **Code Review**: Address feedback from code review
8. **Merge**: After approval, merge your changes

### Commit Messages

We follow the Conventional Commits specification:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

Types:
- **feat**: A new feature
- **fix**: A bug fix
- **docs**: Documentation changes
- **style**: Changes that do not affect the meaning of the code
- **refactor**: Code changes that neither fix a bug nor add a feature
- **perf**: Performance improvements
- **test**: Adding or correcting tests
- **chore**: Changes to the build process or auxiliary tools

Examples:
- `feat(auth): add multi-factor authentication`
- `fix(api): resolve project creation error`
- `docs(readme): update installation instructions`

### Code Style

Please follow our [coding standards](coding-standards.md) for all contributions.

## Testing Guidelines

- Write unit tests for all new code
- Ensure all existing tests pass
- Add integration tests for API endpoints
- Add end-to-end tests for user flows
- Aim for high test coverage

## Documentation

- Update documentation when adding or changing features
- Document public APIs with XML comments or JSDoc
- Update README or other guides as needed
- Add code comments where necessary to explain complex logic

## Community

### Communication Channels

- **GitHub Issues**: For bug reports and feature requests
- **Discord**: For general discussion and questions
- **Slack**: For team communication (invite required)
- **Mailing List**: For announcements and newsletters

### Team Meetings

- **Sprint Planning**: Every two weeks
- **Daily Stand-up**: Every weekday
- **Sprint Review**: End of each sprint
- **Retrospective**: After each sprint

### Recognition

We value all contributions to OuiAI and recognize contributors in:

- Release notes
- Contributors list
- Monthly highlights

## License

By contributing to OuiAI, you agree that your contributions will be licensed under the project's [license](../../LICENSE).
