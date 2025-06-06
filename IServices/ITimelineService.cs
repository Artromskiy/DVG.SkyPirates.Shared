﻿using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService: ITickable
    {
        int CurrentTick { get; }
        void AddCommand<T>(Command<T> command) where T: ICommandData;
        void RemoveCommand(int clientId, int commandId);
    }
}
