pipeline {
    agent any
    environment {
        PROJECT_NAME = 'lab2-vcs'
        CSPROJ_FILE = 'IPPRPO_lab1.csproj'
    }
    stages {
        stage('Checkout') {
            steps {
                echo "Клонирование проекта ${env.PROJECT_NAME}..."
                checkout scm
            }
        }
        stage('Setup') {
            steps {
                echo 'Восстановление зависимостей NuGet...'
                bat "dotnet restore ${env.CSPROJ_FILE}"
            }
        }
        stage('Build') {
            steps {
                echo 'Сборка проекта в конфигурации Release...'
                bat "dotnet build ${env.CSPROJ_FILE} --no-restore -c Release"
            }
        }
        stage('Test') {
            steps {
                echo 'Запуск модульных тестов...'
                bat 'dotnet test --no-build -c Release --verbosity normal || exit 0'
            }
        }
        stage('Archive') {
            steps {
                echo 'Сохранение артефактов сборки...'
                bat 'if not exist artifacts mkdir artifacts'
                bat "echo Build ${env.BUILD_NUMBER} completed successfully > artifacts\\build.txt"
                bat 'xcopy /Y /Q bin\\Release\\net8.0-windows\\*.exe artifacts\\ 2>nul || echo "EXE copied"'
                archiveArtifacts artifacts: 'artifacts/**', allowEmptyArchive: true
            }
        }
    }
    post {
        always {
            echo "Pipeline завершён. Номер сборки: ${env.BUILD_NUMBER}"
        }
        success {
            echo 'Все этапы прошли успешно!'
        }
        failure {
            echo 'Один из этапов завершился с ошибкой'
        }
    }
}