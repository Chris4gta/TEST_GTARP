module.exports = function(grunt) {

    grunt.initConfig({
        sass: {
            dist: {
                files: {
                    'UI/general/Styles/bootstrap/bootstrap.css': 'UI/general/Styles/bootstrap/bootstrap.scss',
                    'UI/custom/styles.css': 'UI/custom/styles.scss'
                }
            }
        }
    });

    grunt.loadNpmTasks('grunt-sass');
    grunt.registerTask('default', ['sass']);
};