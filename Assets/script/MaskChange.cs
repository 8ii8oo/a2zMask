using UnityEngine;
using Spine.Unity;
// 🚨 추가된 부분: Spine 핵심 런타임 접근을 위함 🚨
using Spine; 

public class MaskChange : MonoBehaviour
{
    // 유니티 인스펙터에 할당 (필수)
    public SkeletonGraphic skeletonGraphic;

    private const string SLOT_NAME = "black_mask";
    private const string BLACK_MASK_ATTACHMENT = "black_mask";
    private const string BLUE_MASK_ATTACHMENT = "blue_mask"; 
    private const string RED_MASK_ATTACHMENT = "red_mask";   

    void Start()
    {
        if (skeletonGraphic == null)
        {
            skeletonGraphic = GetComponent<SkeletonGraphic>();
        }
        
        if (skeletonGraphic == null)
        {
            Debug.LogError("SkeletonGraphic 컴포넌트를 찾을 수 없습니다.");
        }
    }    

    public void SetMaskAttachment(string attachmentName)
    {
        if (skeletonGraphic == null || skeletonGraphic.Skeleton == null) 
        {
            Debug.LogError("SkeletonGraphic 컴포넌트 또는 스켈레톤 데이터가 준비되지 않았습니다.");
            return;
        }

        // 1. 슬롯 찾기
        // Spine.Slot 클래스는 using Spine; 덕분에 접근 가능
        Spine.Slot slot = skeletonGraphic.Skeleton.FindSlot(SLOT_NAME);

        
    }

    public void ChangeToBlueMask()
    {
        SetMaskAttachment(BLUE_MASK_ATTACHMENT);
    }
    
    public void ChangeToRedMask()
    {
        SetMaskAttachment(RED_MASK_ATTACHMENT);
    }

    public void ChangeToBlackMask()
    {
        SetMaskAttachment(BLACK_MASK_ATTACHMENT);
    }
    
    // SetCharacterSkin 함수는 생략했습니다.
}