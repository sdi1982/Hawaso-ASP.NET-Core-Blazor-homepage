﻿using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using VisualAcademy.Models.Libraries;

namespace Hawaso.Pages.Libraries
{
    public partial class Edit
    {
        [Parameter]
        public int Id { get; set; }

        [Inject]
        public ILibraryRepository UploadRepositoryAsyncReference { get; set; }

        [Inject]
        public NavigationManager NavigationManagerReference { get; set; }

        protected LibraryModel model = new LibraryModel();

        public string ParentId { get; set; }

        protected int[] parentIds = { 1, 2, 3 };

        protected string content = "";

        protected override async Task OnInitializedAsync()
        {
            model = await UploadRepositoryAsyncReference.GetByIdAsync(Id);
            content = Dul.HtmlUtility.EncodeWithTabAndSpace(model.Content);
            ParentId = model.ParentId.ToString();
        }

        protected async void FormSubmit()
        {
            int.TryParse(ParentId, out int parentId);
            model.ParentId = parentId;

            #region 파일 업로드 관련 추가 코드 영역
            if (selectedFiles != null && selectedFiles.Length > 0)
            {
                // 파일 업로드
                var file = selectedFiles.FirstOrDefault();
                var fileName = "";
                int fileSize = 0;
                if (file != null)
                {
                    fileName = file.Name;
                    fileSize = Convert.ToInt32(file.Size);

                    // 첨부 파일 삭제 
                    await FileStorageManager.DeleteAsync(model.FileName, "Libraries");

                    // 다시 업로드
                    fileName = await FileStorageManager.UploadAsync(file.Data, file.Name, "", true);

                    model.FileName = fileName;
                    model.FileSize = fileSize;
                }
            }
            #endregion

            await UploadRepositoryAsyncReference.EditAsync(model);
            NavigationManagerReference.NavigateTo("/Libraries");
        }

        [Inject]
        public ILibraryFileStorageManager FileStorageManager { get; set; }
        private IFileListEntry[] selectedFiles;
        protected void HandleSelection(IFileListEntry[] files)
        {
            this.selectedFiles = files;
        }
    }
}
