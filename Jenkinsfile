#!/usr/bin/env groovy

pipeline {
    agent any

    options {
        buildDiscarder(logRotator(numToKeepStr: '10'))
        disableConcurrentBuilds()
        timeout(time: 1, unit: 'HOURS')
        timestamps()
    }

    environment {
        VERSION = '${BUILD_NUMBER}'

        ACCOUNT_ID = '515001955773'
        AWS_ECR_REGION = 'us-east-2'
        AWS_URL = '515001955773.dkr.ecr.us-east-2.amazonaws.com'
        IMAGE_NAME = '515001955773.dkr.ecr.us-east-2.amazonaws.com/cloudx-products'
        TASK_NAME = 'CloudX-container'
        CLUSTER_NAME = 'ECS-CloudXCluster'
        SERVICE_NAME = 'CloudXService'
    }

    stages {
    stage('Clone repository') {
            steps {
                git branch: "main", url: "https://github.com/zhurinvlad/CloudXProductShop.git"
            }
        }
    stage('Tests')
    {
      steps {
                sh 'echo tests'
      }
    }
    stage('Build image') {
      steps {
            sh 'cd CloudXProductShop && docker build -t ${IMAGE_NAME} -f ./CloudXProductShop/Dockerfile .'
      }
    } 

    stage('Push image') {
      steps {
        script {
          docker.withRegistry('https://515001955773.dkr.ecr.us-east-2.amazonaws.com', 'ecr:us-east-2:jenkins-ecr') {
            docker.image('515001955773.dkr.ecr.us-east-2.amazonaws.com/cloudx-products').push("v${env.BUILD_NUMBER}")
            docker.image('515001955773.dkr.ecr.us-east-2.amazonaws.com/cloudx-products').push('latest')
          }
        }
      }
    }
    stage('Run instance')
    {
        //  sh 'sed -e "s;%BUILD_NUMBER%;${BUILD_NUMBER};g" ${TASK_NAME}.json >  ${TASK_NAME}-v${BUILD_NUMBER}.json'
        //  sh 'aws ecs register-task-definition --cli-input-json file://${TASK_NAME}-v${BUILD_NUMBER}.json --region ${AWS_ECR_REGION}'
      steps {
         sh '''  
            TASK_REVISION=`aws ecs describe-task-definition --task-definition ${TASK_NAME} --region ${AWS_ECR_REGION} | egrep "revision" | tr "/" " " | awk '{print $2}' | sed 's/"$//'`
            aws ecs update-service --cluster ${CLUSTER_NAME} --service ${SERVICE_NAME} --task-definition ${TASK_NAME}:1 --desired-count 1 --region ${AWS_ECR_REGION}
        '''
      }
    }
    stage('Clean up')
    {    
      steps {
        sh 'docker image prune -a -f'
        deleteDir()
        script {
          currentBuild.result = 'SUCCESS'
        }
      }
    }
    }

    post {
        failure {
            script {
                currentBuild.result = 'FAILURE'
            }
        }
    }
    
}
