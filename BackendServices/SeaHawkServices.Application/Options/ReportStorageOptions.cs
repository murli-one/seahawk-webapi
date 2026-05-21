using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SeaHawkServices.Application.Options
{
    public sealed class ReportStorageOptions
    {
        public string ReportsRoot { get; set; } = "";
        public string AccessToken { get; set; } = "";

        //public string ReportsRoot { get; set; } = "/";
        //public string AccessToken { get; set; } = "sl.u.AGRizX_TRN7wNFKpwsOQRJZc5XJQDhyOVZYmY2DgB6Hp1JeS-Fn-J1ZmUpKZwcr0QG-B3jJdbJVDS78Bsp5FLVASmdH3_cphs5jVB5nquX2Eqz56mAICcaRtbjx-0pzISXR0fIynqWoUd5TY7Lk-ujRrFHaokoFhdNOu9R4ZzU2Ft1B08-NVVFLMPEe0wO4xtKGJnsDRrW9QU4llt_wlvQP8aRwt9DNMdHEPWnW5-am0JDADRmFS4cLBEvhocMFBCre4_6zEphrJr5cKIwfFVhRX1HNuIt1FbztpaIXosYVoF1iSpO5PSzHsdfwJ5-k_XxskVznWivW5l77bO0wMLeFgoFRoFiZMFjaGXONemSiu4hB4l2oa92docbgPeNXWbphBhXIKorJvL2LvVCV-f2FheXRzZyBfBtS2y0kOP0SSNPszCVpQxOK1GKXd4EqPbtXaAUNC6Z04dUw0jS6iwaIpuOi5LcCWoS1p-OMjcNN5SGlYjGwWhBza-ydjFoM-u8j872agkqkYFJqQ1dDqPajTlG6R-fAoAv6JC0ZdcOP0-Op2kwFaUjJeLwPy6ZqUJeP6xZ8obYuIgSFyVLrVL3mmH7awOPN8d6RMWA9AK1pP3xI-BZ_bpXfFs8ASQPASkNmf4b3dLv2hJEbhW7vql-UDAp6qKLgaUU7k5K7yTW2ODCC93fjcfYil42tP4Pdj7nHYCIcoVF2tHnsgVa7izTU8OVJGqMyp8gQwSMq9kFZlA184k2YPRbIQ_WDKelxD8M4sxwYam1N0pzeBlCVQMI96OuQpS99yfPs5UlaJe6qhbsJMQpzNKFxCFWqC5_-NRg84avM-3EgSRlhioigxL9gWbeajFZ5HAqKdI5Q_ilOft_Cthgzo_8zE50XJhdFL3UMhVx3VguL87HgFQhk5tgIcyKUO_cIAF-zIp3pOINI-cV1mYCbSRyC-3jd8-e2cobQCNDHD38wTDMNVY8KVxha-w5DIsnk2vE5lkBie51XZ03rkDtnt9bdfRuAM7VjnZHrW8pd-NgAH3qMFdysRMJC96ppAVQ63hDbj8-_zGDmpobuzTmE2Owfv_vHcblu3MY_oeyyD-HliLQD8jKuTDcWNjQd78qQqad-zLaVQfpN8LbljuxL6xxZ6qpcvM1K0ow8CHTvgA2RYy-3-yYcOs52fqXjK33Y7JijO3oD0UgZqpLtHVCEwOYqFRBbK_o-jRYBSDxYouclcp-aKfnalAYrM8txZNneUzXmo8cYL77QzgH9b-g3awLvcLHwK42nPZT3qJhish0VQPpgdsPtuqS3N4FBHxLshPkfmwHWcVFUt6A";

        // Required for refresh flow
        // final refresh token will be stored here after one-time exchange
        public string RefreshToken { get; set; } = "H4VK-H3ZUsUAAAAAAAAAAUQrV5qYuBWrVBa_0B5Ub8_-_aKLXMMw0BM4EF-rISso";

        // required for one-time auth code exchange
        public string RedirectUri { get; set; } = "https://ak.mt.cisinlive.com/ddts";
        public string AppKey { get; set; } = "exao5i5aqb81tbw";
        public string AppSecret { get; set; } = "15hukkq7yb3ki87";
    }
   

}
