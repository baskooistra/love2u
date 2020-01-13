module.exports = {
  name: 'material-ui',
  preset: '../../../jest.config.js',
  coverageDirectory: '../../../coverage/libs/ui/material',
  snapshotSerializers: [
    'jest-preset-angular/AngularSnapshotSerializer.js',
    'jest-preset-angular/HTMLCommentSerializer.js'
  ]
};
