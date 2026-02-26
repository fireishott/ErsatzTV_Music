using System;

namespace ErsatzTV.Core.Interfaces
{
    public interface IEntityLocker
    {
        bool LockLibrary(int libraryId);
        void UnlockLibrary(int libraryId);
        bool LockCollection(int collectionId);
        void UnlockCollection(int collectionId);
        bool LockPlayout(int playoutId);
        void UnlockPlayout(int playoutId);
        bool LockChannel(int channelId);
        void UnlockChannel(int channelId);
        bool LockTemplate(int templateId);
        void UnlockTemplate(int templateId);
        bool LockWatermark(int watermarkId);
        void UnlockWatermark(int watermarkId);
    }
}
