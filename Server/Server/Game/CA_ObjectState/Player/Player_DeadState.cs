using Google.Protobuf.Protocol;
using Server.Game.CA_Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

 
public class Player_DeadState : AbstractPlayerState
{
    public override void ApplyMove(InGameObject gameObject, MoveDir dir)
    {
        // 죽은 상태에서는 움직일 수 없다. 
        return;
    }

    public override void EnterState(InGameObject obj, IObjectState previousState)
    {
        // 에니메이션 변경.
        S_ChangeAnimation changeAnimPkt = new S_ChangeAnimation
        {
            ObjectId = obj.Id,
            PlayerAnim = PlayerAnimState.PlayerAnimDead,
        };

        obj._possessGame._gameRoom.BroadcastPacket(changeAnimPkt);


        // 캐릭터 사망 사운드 재생.
        S_PlaySoundEffect deadSoundEffect = new S_PlaySoundEffect
        {
            SoundEffectType = SoundEffectType.PlayerDieSoundEffect
        };

        obj._possessGame._gameRoom.BroadcastPacket(deadSoundEffect);

        // 게임이 끝났는지 확인
        if (obj._possessGame.IsGameFinished())
        {
            Console.WriteLine("GAME FINISHED!!");
            CharacterType charType = obj._possessGame.CalculateWinnerTeam();

            if (charType == CharacterType.CharacterNone)
            {
                obj._possessGame.FinishGame(charType, true);
            }

            else
            {
                obj._possessGame.FinishGame(charType, false);
            }
        }
    }

    public override void UpdateState(InGameObject gameObject)
    {

    }


    public override void ExitState(InGameObject obj)
    {

    }
}
