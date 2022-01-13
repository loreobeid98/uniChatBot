using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot
{
    public static class GlobalConstantClass
    {
        public static string promptMessage = "Start by typing what you want";
        public static string WelcomeCard = "welcomeCard";
        public static string CourseCard = "courseCard";
        public static string EnrolmentBaseUrl = "https://60ca4227772a760017205add.mockapi.io";
        public static string EnrolmentApiRoute = "/Enrolment";
        public static string StudentInCoursesIntent = "Students in Course";
        public static string CoursesIntent = "q_CoursesKB";
        public static string MajorsIntent = "q_MajorsKB";
        public static string FinalChitChatIntent = "q_FinalChitChatKB";
        public static string StorageQueueConString = "DefaultEndpointsProtocol=https;AccountName=storageaccountazure9f41;AccountKey=dH0mibgnZ8jt4C4EcKcqhULjkIYmrPMEdrCduowQmZOtEAyAnHWjv+NMb7hzrYytHZX6wX27lTFDzfY/d5oLuw==;EndpointSuffix=core.windows.net";
        public static string QueueName = "exceptionqueue";
    }
}
