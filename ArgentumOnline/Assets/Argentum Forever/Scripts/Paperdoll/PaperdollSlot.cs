using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperdollSlot : MonoBehaviour
{
    #region components cache
    private Animator mAnimator;
    private SpriteRenderer mSpriteRenderer;
    private Animator mRootAnimator;
    #endregion

    // Associated slot type
    [SerializeField] public EquipmentSlotType type;

    // Animator overrider
    private AnimatorOverrideController mAnimatorOverride;

    #region unity loop
    // Start is called before the first frame update
    void Awake()
    {
        // Setup the component cache
        SetupComponentCache();        

        // Instantiate the animator override controller
        mAnimatorOverride = new AnimatorOverrideController(mAnimator.runtimeAnimatorController);

        // Actually override the animator controller
        mAnimator.runtimeAnimatorController = mAnimatorOverride;
    }
    #endregion

    private void SetupComponentCache()
    {
        // Set up all the cached components
        mAnimator = GetComponent<Animator>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mRootAnimator = GetComponentInParent<Animator>();
    }

    public void LoadAnimationSet(AnimationClip[] animations)
    {
        // Reset the alpha channel of this slot
        Color newColor = mSpriteRenderer.color;
        newColor.a = 1;
        mSpriteRenderer.color = newColor;

        // Override the animation set
        //TODO generalizar
        mAnimatorOverride["attack_east"] = animations[0];
        mAnimatorOverride["attack_north"] = animations[1];
        mAnimatorOverride["attack_northeast"] = animations[2];
        mAnimatorOverride["attack_northwest"] = animations[3];
        mAnimatorOverride["attack_south"] = animations[4];
        mAnimatorOverride["attack_southeast"] = animations[5];
        mAnimatorOverride["attack_southwest"] = animations[6];
        mAnimatorOverride["attack_west"] = animations[7];
        mAnimatorOverride["run_east"] = animations[8];
        mAnimatorOverride["run_north"] = animations[9];
        mAnimatorOverride["run_northeast"] = animations[10];
        mAnimatorOverride["run_northwest"] = animations[11];
        mAnimatorOverride["run_south"] = animations[12];
        mAnimatorOverride["run_southeast"] = animations[13];
        mAnimatorOverride["run_southwest"] = animations[14];
        mAnimatorOverride["run_west"] = animations[15];
        mAnimatorOverride["stand_east"] = animations[16];
        mAnimatorOverride["stand_north"] = animations[17];
        mAnimatorOverride["stand_northeast"] = animations[18];
        mAnimatorOverride["stand_northwest"] = animations[19];
        mAnimatorOverride["stand_south"] = animations[20];
        mAnimatorOverride["stand_southeast"] = animations[21];
        mAnimatorOverride["stand_southwest"] = animations[22];
        mAnimatorOverride["stand_west"] = animations[23];
        mAnimatorOverride["walk_east"] = animations[24];
        mAnimatorOverride["walk_north"] = animations[25];
        mAnimatorOverride["walk_northeast"] = animations[26];
        mAnimatorOverride["walk_northwest"] = animations[27];
        mAnimatorOverride["walk_south"] = animations[28];
        mAnimatorOverride["walk_southeast"] = animations[29];
        mAnimatorOverride["walk_southwest"] = animations[30];
        mAnimatorOverride["walk_west"] = animations[31];
    }

    public void ResetAnimationSet()
    {
        // Set all the animations for this slot to null (no override)
        //TODO generalizar
        mAnimatorOverride["attack_east"] = null;
        mAnimatorOverride["attack_north"] = null;
        mAnimatorOverride["attack_northeast"] = null;
        mAnimatorOverride["attack_northwest"] = null;
        mAnimatorOverride["attack_south"] = null;
        mAnimatorOverride["attack_southeast"] = null;
        mAnimatorOverride["attack_southwest"] = null;
        mAnimatorOverride["attack_west"] = null;
        mAnimatorOverride["run_east"] = null;
        mAnimatorOverride["run_north"] = null;
        mAnimatorOverride["run_northeast"] = null;
        mAnimatorOverride["run_northwest"] = null;
        mAnimatorOverride["run_south"] = null;
        mAnimatorOverride["run_southeast"] = null;
        mAnimatorOverride["run_southwest"] = null;
        mAnimatorOverride["run_west"] = null;
        mAnimatorOverride["stand_east"] = null;
        mAnimatorOverride["stand_north"] = null;
        mAnimatorOverride["stand_northeast"] = null;
        mAnimatorOverride["stand_northwest"] = null;
        mAnimatorOverride["stand_south"] = null;
        mAnimatorOverride["stand_southeast"] = null;
        mAnimatorOverride["stand_southwest"] = null;
        mAnimatorOverride["stand_west"] = null;
        mAnimatorOverride["walk_east"] = null;
        mAnimatorOverride["walk_north"] = null;
        mAnimatorOverride["walk_northeast"] = null;
        mAnimatorOverride["walk_northwest"] = null;
        mAnimatorOverride["walk_south"] = null;
        mAnimatorOverride["walk_southeast"] = null;
        mAnimatorOverride["walk_southwest"] = null;
        mAnimatorOverride["walk_west"] = null;

        // Reset the alpha channel
        Color newColor = mSpriteRenderer.color;
        newColor.a = 0;
        mSpriteRenderer.color = newColor;
    }

    public void UpdateAnimatorFlags(bool directionChanged, float horizontalSpeed, float verticalSpeed, float finalSpeed)
    {
        if (directionChanged)
        {
            mAnimator.SetFloat("Horizontal", horizontalSpeed);
            mAnimator.SetFloat("Vertical", verticalSpeed);
        }        
        mAnimator.SetFloat("Speed", finalSpeed);      
    }

    public void UpdateMeleeAttackStatus(bool started)
    {
        // Attack started?
        if (started)
        {
            mAnimator.SetTrigger("DoMeleeAttack");
        }

        // Attack finished
        else
        {
            ///TODO es necesario hacer algo aca?
        }
    }
}
